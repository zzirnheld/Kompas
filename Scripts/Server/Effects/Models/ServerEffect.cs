using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Server.Effects.Models;

public interface IServerEffect : IEffect, IServerStackable
{
	public ServerSubeffect[] ServerSubeffects { get; }

	public void SetInfo(ServerGameCard card, IServerGame game, int index);

	public bool CanBeActivatedBy(IPlayer player);

	public void PushedToStack(ServerPlayer controller, string blurb);
}

public class ServerEffect : Effect, IServerEffect
{
	private IServerGame? _serverGame;
	public IServerGame ServerGame => _serverGame
		?? throw new NotInitializedException();
	public override IGame Game => ServerGame;
	public IServerStackController EffectsController => ServerGame.StackController;

	private ServerGameCard? _card;
	public override GameCard Card => _card
		?? throw new NotInitializedException();

	private ServerPlayer? _ownerServerPlayer;
	public ServerPlayer OwningServerPlayer => _ownerServerPlayer
		?? throw new NotInitializedException();
	public override IPlayer OwningPlayer => OwningServerPlayer;

	public ServerSubeffect[] ServerSubeffects { get; private set; }
	public override Subeffect[] Subeffects => ServerSubeffects;
	public ServerTrigger? ServerTrigger { get; private set; }
	public override Trigger? Trigger => ServerTrigger;

	public override bool Negated
	{
		get => base.Negated;
		set
		{
			//If being negated, cancel anywhere this was on the stack, and cancel any hanging effects for this
			_ = EffectsController ?? throw new System.InvalidOperationException("No effects controller!?");
			if (!Negated && !value) EffectsController.Cancel(this);
			base.Negated = value;
		}
	}

	public ServerEffect(EffectData data) : base(data)
	{
		ServerSubeffects = data.Subeffects
			.Select(sd => FACTORY_METHOD)
			.ToArray();

		if (data.triggerData != null && !string.IsNullOrEmpty(data.triggerData.triggerCondition))
			ServerTrigger = ServerTrigger.Create(data.triggerData, this);
	}

	public void SetInfo(ServerGameCard card, IServerGame game, int effectIndex)
	{
		_card = card;
		_serverGame = game;
		_ownerServerPlayer = game.ServerControllerOf(card);
		base.SetInfo(effectIndex);

		foreach (var (i, subeff) in ServerSubeffects.Enumerate()) subeff.Initialize(this, i);
	}

	/// <summary>
	/// Inserts the given array of subeffects into this effect's <see cref="ServerSubeffects"/> array.
	/// The first subeffect (index 0) of <paramref name="newSubeffects"/> 
	/// will be at <paramref name="startingAtIndex"/> in the new array.
	/// </summary>
	/// <param name="startingAtIndex"></param>
	/// <param name="newSubeffects"></param>
	public void InsertSubeffects(int startingAtIndex, params ServerSubeffect[] newSubeffects)
	{
		if (newSubeffects == null) throw new System.ArgumentNullException(nameof(newSubeffects), "Can't insert null subeffects");

		//First, update the subeffect jump indices
		//Of the subeffects to be inserted
		foreach (var s in newSubeffects) s.AdjustSubeffectIndices(startingAtIndex);
		//And of any extant subeffects whose indices would be after the insertion point
		foreach (var s in ServerSubeffects) s.AdjustSubeffectIndices(newSubeffects.Length, startingAtIndex);

		ServerSubeffect[] combinedSubeffects = new ServerSubeffect[ServerSubeffects.Length + newSubeffects.Length];
		int oldIndex;
		int newIndex;
		int combinedIndex;
		//Add old subeffects to combined array, until you get to the index where you want to insert the new ones
		for (oldIndex = 0, combinedIndex = 0;
			combinedIndex < startingAtIndex;
			oldIndex++, combinedIndex++)
		{
			combinedSubeffects[combinedIndex] = ServerSubeffects[oldIndex];
		}
		//Add all the new subeffects to the combined array
		for (newIndex = 0;
			newIndex < newSubeffects.Length;
			newIndex++, combinedIndex++)
		{
			combinedSubeffects[combinedIndex] = newSubeffects[newIndex];
		}
		//Add the remaining old subeffects to the array
		for (;
			oldIndex < ServerSubeffects.Length;
			oldIndex++, combinedIndex++)
		{
			combinedSubeffects[combinedIndex] = ServerSubeffects[oldIndex];
		}
		ServerSubeffects = combinedSubeffects;
	}

	public override bool CanBeActivatedBy(IPlayer controller)
		=> (ServerGame?.DebugMode ?? false)
		|| base.CanBeActivatedBy(controller);

	public void PushedToStack(ServerPlayer controller, string blurb)
	{
		var contexts = IEventContext.Build(Trigger.EffectPushedToStack)
			.CausedBy(this)
			.During(this)
			.Capture(() => { });
		EffectsController.TriggerFor(contexts);
		TimesUsedThisRound++;
		TimesUsedThisTurn++;
		TimesUsedThisStack++;
		// _serverGame = game; //TODO: verify commenting this out doesn't break anything
		ServerNotifier.NotifyEffectActivated(controller, this, blurb);
	}

	public void CreateCardLink(Color linkColor, IPlayer? onlyPlayerToKnow = null, params GameCard[] cards)
	{
		GameCard[] validCards = cards.Where(c => c != null).ToArray();
		//if (validCards.Length <= 1) return; //Don't create a link between one non-null card? nah, do, so we can delete it as expected later

		var link = new CardLink(new HashSet<int>(validCards.Select(c => c.ID)), this, linkColor);
		cardLinks.Add(link);
		if (onlyPlayerToKnow != null) ServerNotifier.AddHiddenCardLink(onlyPlayerToKnow, link);
		else ServerNotifier.AddCardLink(link, Game.Players);
	}

	public void DestroyCardLink(int index)
	{
		var link = cardLinks.ElementAtWrapped(index);
		if (link == null)
		{
			Logger.Err($"No card link at index {index}");
			return;
		}

		if (cardLinks.Remove(link))
		{
			ServerNotifier.RemoveCardLink(link, Game.Players);
		}
	}

	public override string ToString() => $"Effect {EffectIndex} of {_card?.CardName}";

	public async Task StartResolution(IServerResolutionContext context)
	{
		await new ServerEffectResolution(this, context).StartResolution();
    }
}