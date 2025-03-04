using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
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

	public void PushedToStack(ServerGame serverGame, ServerPlayer controller);
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

	public IServerResolutionContext? CurrentServerResolutionContext { get; private set; }
	public override IResolutionContext? CurrentResolutionContext => CurrentServerResolutionContext;

	public ServerSubeffect[] subeffects = Array.Empty<ServerSubeffect>();
	public ServerSubeffect[] ServerSubeffects => subeffects;
	public override Subeffect[] Subeffects => subeffects;
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

	public void SetInfo(ServerGameCard card, IServerGame game, int effectIndex)
	{
		_card = card;
		_serverGame = game;
		_ownerServerPlayer = game.ServerControllerOf(card);
		base.SetInfo(effectIndex);

		if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
			ServerTrigger = ServerTrigger.Create(triggerData, this);

		foreach (var (i, subeff) in subeffects.Enumerate()) subeff.Initialize(this, i);
	}

	/// <summary>
	/// Inserts the given array of subeffects into this effect's <see cref="subeffects"/> array.
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
		foreach (var s in subeffects) s.AdjustSubeffectIndices(newSubeffects.Length, startingAtIndex);

		ServerSubeffect[] combinedSubeffects = new ServerSubeffect[subeffects.Length + newSubeffects.Length];
		int oldIndex;
		int combinedIndex;
		//Add old subeffects to combined array, until you get to the index where you want to insert the new ones
		for (oldIndex = 0, combinedIndex = 0; combinedIndex < startingAtIndex; oldIndex++, combinedIndex++)
		{
			combinedSubeffects[combinedIndex] = subeffects[oldIndex];
		}
		//Add all the new subeffects to the combined array
		for (int newIndex = 0; newIndex < newSubeffects.Length; newIndex++, combinedIndex++)
		{
			combinedSubeffects[combinedIndex] = newSubeffects[newIndex];
		}
		//Add the remaining old subeffects to the array
		for (; oldIndex < subeffects.Length; oldIndex++, combinedIndex++)
		{
			combinedSubeffects[combinedIndex] = subeffects[oldIndex];
		}
		subeffects = combinedSubeffects;
	}

	public override bool CanBeActivatedBy(IPlayer controller)
		=> (ServerGame?.DebugMode ?? false)
		|| base.CanBeActivatedBy(controller);

	public void PushedToStack(ServerGame game, ServerPlayer controller)
	{
		var contexts = IEventContext.Build(Trigger.EffectPushedToStack)
			.CausedBy(this)
			.During(this)
			.Capture(() => { });
		EffectsController.TriggerFor(contexts);
		TimesUsedThisRound++;
		TimesUsedThisTurn++;
		TimesUsedThisStack++;
		_serverGame = game;
		ServerNotifier.NotifyEffectActivated(controller, this);
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