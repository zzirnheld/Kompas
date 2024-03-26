using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
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

namespace Kompas.Server.Effects.Models
{
	public interface IServerEffect : IEffect, IServerStackable
	{
		public void SetInfo(ServerGameCard card, IServerGame game, int index);

		public bool CanBeActivatedBy(IPlayer player);

		public void PushedToStack(ServerGame serverGame, ServerPlayer controller);
	}

	public class ServerEffect : Effect, IServerEffect
	{
		public const string EffectWasNegated = "Effect was negated";

		private IServerGame? _serverGame;
		public IServerGame ServerGame => _serverGame
			?? throw new NotInitializedException();
		public override IGame Game => ServerGame;
		public ServerStackController EffectsController => ServerGame.StackController;

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
		public override Subeffect[] Subeffects => subeffects;
		public ServerTrigger? ServerTrigger { get; private set; }
		public override Trigger? Trigger => ServerTrigger;

		public ServerSubeffect? OnImpossible { get; set; } = null;
		public bool CanDeclineTarget = false;

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
			=> ServerGame?.DebugMode ?? false || base.CanBeActivatedBy(controller);

		public void PushedToStack(ServerGame game, ServerPlayer controller)
		{
			TriggeringEventContext context = new(game: game, stackableCause: this, stackableEvent: this);
			EffectsController.TriggerForCondition(Trigger.EffectPushedToStack, context);
			TimesUsedThisRound++;
			TimesUsedThisTurn++;
			TimesUsedThisStack++;
			_serverGame = game;
			ServerNotifier.NotifyEffectActivated(controller, this);
		}

		#region resolution
		public async Task StartResolution(IServerResolutionContext context)
		{
			GD.Print($"Resolving effect {EffectIndex} of {Card.CardName} in context {context}");

			//set context parameters
			CurrentServerResolutionContext = context;
			//Notify the targets one by one so the client knows that they're current targets
			if (context.CardTargets != null) foreach(var tgt in context.CardTargets) NotifyAddCardTarget(tgt);
			
			playerTargets.Add(context.ControllingPlayer);
			if (context.TriggerContext?.StackableCause != null) StackableTargets.Add(context.TriggerContext.StackableCause);

			//notify relevant to this effect starting
			ServerNotifier.NotifyEffectX(Card, EffectIndex, X, Game.Players);
			ServerNotifier.EffectResolving(context.ControllingPlayer,this);

			//resolve the effect if possible
			if (Negated) await EffectImpossible(EffectWasNegated);
			else await Resolve(context.StartIndex);

			//after all subeffects have finished, clean up
			FinishResolution();

			//then return. server effects controller will interpret returning as effect being done.
		}

		private async Task Resolve(int index)
		{
			//get first result
			ResolutionInfo result = await ResolveSubeffect(index);

			//then, so long as we should keep going, resolve subeffects
			bool resolve = true;
			while (resolve)
			{
				switch (result.result)
				{
					case ResolutionResult.Next:
						index++;
						if (index < subeffects.Length) result = await ResolveSubeffect(index);
						else resolve = false; //stop if next subeffect index is out of bounds
						break;
					case ResolutionResult.Index:
						index = result.index;
						if (index < subeffects.Length) result = await ResolveSubeffect(index);
						else resolve = false; //stop if that subeffect index is out of bounds
						break;
					case ResolutionResult.Impossible:
						GD.Print($"Effect of {Card?.CardName} was impossible at index {index} because {result.reason}. Going to OnImpossible if applicable");
						result = await EffectImpossible(result.reason);
						break;
					case ResolutionResult.End:
						//TODO send to player why resolution ended (including "[cardname] effect finished resolving")
						GD.Print($"Finished resolution of effect of {Card?.CardName} because {result.reason}");
						resolve = false;
						break;
					default:
						throw new System.ArgumentException($"Invalid resolution result {result.result}");
				}
			}
		}

		public async Task<ResolutionInfo> ResolveSubeffect(int index)
		{
			if (index >= subeffects.Length)
			{
				return ResolutionInfo.Impossible("Subeffect index out of bounds.");
			}
			GD.Print($"Resolving subeffect of type {subeffects[index].GetType()}");
			SubeffectIndex = index;
			ServerNotifier.NotifyEffectX(Card, EffectIndex, X, Game.Players);
			try
			{
				return await subeffects[index].Resolve();
			}
			catch (KompasException e)
			{
				GD.PushWarning($"Caught {e.GetType()} while resolving {subeffects[index].GetType()} at {index}." +
					$"\nStack trace:\n{e.StackTrace}");
				return ResolutionInfo.Impossible(e.Message);
			}
		}

		/// <summary>
		/// If the effect finishes resolving, this method is called.
		/// </summary>
		private void FinishResolution()
		{
			SubeffectIndex = 0;
			_ = CurrentResolutionContext ?? throw new EffectNotResolvingException(this);
			CurrentResolutionContext.X = 0;
			CardTargets.Clear();
			rest.Clear();
			OnImpossible = null;
			Networking.ServerNotifier.NotifyBothPutBack(Game.Players);
		}

		/// <summary>
		/// Cancels resolution of the effect, 
		/// or, if there is something pending if the effect becomes impossible, resolves that
		/// </summary>
		public async Task<ResolutionInfo> EffectImpossible(string why)
		{
			GD.Print($"Effect of {Card.CardName} is being declared impossible at subeffect {subeffects[SubeffectIndex].GetType()} because {why}");
			if (OnImpossible == null)
			{
				//TODO make the notifier tell the client why the effect was impossible
				ServerNotifier.EffectImpossible(Game.Players);
				return ResolutionInfo.End(ResolutionInfo.EndedBecauseImpossible);
			}
			else
			{
				SubeffectIndex = OnImpossible.SubeffIndex;
				return await OnImpossible.OnImpossible(why);
			}
		}
		#endregion resolution

		public override void AddTarget(GameCard card) => AddTarget(card);

		public void AddTarget(GameCard card, IPlayer? onlyOneToKnow = null)
		{
			base.AddTarget(card);
			NotifyAddCardTarget(card, onlyOneToKnow);
		}

		private void NotifyAddCardTarget(GameCard card, IPlayer? onlyOneToKnow = null)
		{
			if (onlyOneToKnow != null) ServerNotifier.AddHiddenTarget(onlyOneToKnow, Card, EffectIndex, card);
			else ServerNotifier.AddTarget(Card, EffectIndex, card, Game.Players);
		}

		public override void RemoveTarget(GameCard card)
		{
			base.RemoveTarget(card);
			ServerNotifier.RemoveTarget(Card, EffectIndex, card, Game.Players);
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
				GD.PushError($"No card link at index {index}");
				return;
			}

			if (cardLinks.Remove(link))
			{
				ServerNotifier.RemoveCardLink(link, Game.Players);
			}
		}

		public override string ToString() => $"Effect {EffectIndex} of {_card?.CardName}";
	}
}