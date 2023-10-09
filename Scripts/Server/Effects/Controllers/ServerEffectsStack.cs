using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Kompas.Server.Effects.Models;
using Kompas.Effects.Models;
using Kompas.Effects;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Godot;
using Kompas.Gamestate;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;

namespace Kompas.Server.Effects.Controllers
{
	public class ServerStackController : IStackController
	{
		private struct TriggersTriggered
		{
			public IEnumerable<ServerTrigger> triggers;
			public TriggeringEventContext context;

			public TriggersTriggered(IEnumerable<ServerTrigger> triggers, TriggeringEventContext context)
			{
				this.triggers = triggers;
				this.context = context;
			}
		}

		private readonly ServerGame ServerGame;

		private readonly EffectStack<IServerStackable, IServerResolutionContext> stack = new();
		public IEnumerable<IServerStackable> StackEntries => stack.StackEntries;
		IEnumerable<IStackable> IStackController.StackEntries => StackEntries;

		//queue of triggers triggered throughout the resolution of the effect, to be ordered after the effect resolves
		private readonly Queue<TriggersTriggered> triggeredTriggers = new();

		//trigger maps
		private readonly Dictionary<string, List<ServerTrigger>> triggerMap = new();
		private readonly Dictionary<string, List<HangingEffect>> hangingEffectMap = new();
		private readonly Dictionary<string, List<HangingEffect>> hangingEffectFallOffMap = new();

		private bool currentlyCheckingResponses = false;
		private bool currentlyCheckingOptionals = false;
		private int currStackIndex;
		private IServerStackable _currStackEntry;
		public IServerStackable CurrStackEntry
		{
			get => _currStackEntry;
			private set
			{
				_currStackEntry = value;
				currStackIndex = stack.Count;
			}
		}
		IStackable IStackController.CurrStackEntry => CurrStackEntry;

		//nothing is happening if nothing is in the stack, nothing is currently resolving, and no one is waiting to add something to the stack.
		public bool NothingHappening
			=> stack.Empty
			&& CurrStackEntry == null
			&& !currentlyCheckingResponses
			&& !currentlyCheckingOptionals;


		public override string ToString()
		{
			if (CurrStackEntry == null) return string.Empty;

			StringBuilder sb = new();
			sb.AppendLine("Stack:");
			foreach (var s in stack.StackEntries)
			{
				sb.AppendLine(s.ToString());
			}

			sb.AppendLine("Currently Resolving:");
			sb.AppendLine(CurrStackEntry.ToString());
			if (CurrStackEntry is ServerEffect se)
			{
				if (se.CardTargets.Any())
				{
					sb.Append("Targets: ");
					sb.AppendLine(string.Join(", ", se.CardTargets.Select(c => c.ToString())));
				}
				if (se.SpaceTargets.Any())
				{
					sb.Append("Coords: ");
					sb.AppendLine(string.Join(", ", se.SpaceTargets.Select(c => c.ToString())));
				}
				sb.AppendLine($"X: {se.X}");
			}
			return sb.ToString();
		}

		#region the stack
		//TODO fix these signatures

		public void PushToStack(IServerStackable atk, ServerPlayer controller, TriggeringEventContext triggerContext)
		{
			PushToStack(atk, new ServerResolutionContext(triggerContext, controller));
		}

		public void PushToStack(ServerEffect eff, ServerPlayer controller, TriggeringEventContext triggerContext)
		{
			PushToStack(eff, controller, new ServerResolutionContext(triggerContext, controller));
		}

		public void PushToStack(ServerEffect eff, ServerPlayer controller, IServerResolutionContext context)
		{
			eff.PushedToStack(ServerGame, controller);
			PushToStack(eff, context);
		}

		public void PushToStack(IServerStackable eff, IServerResolutionContext context)
		{
			stack.Push((eff, context));
		}

		private async Task StackEmptied()
		{
			//GD.Print($"Stack is emptied");
			//stack ends
			foreach (var c in ServerGame.Cards) c.ResetForStack();
			ClearSpells();
			ServerGame.Notifier.StackEmpty();
			TriggerForCondition(Trigger.StackEnd, new TriggeringEventContext(game: ServerGame));
			//Must check whether I *should* check for response to avoid an infinite loop
			if (!stack.Empty || triggeredTriggers.Any()) await CheckForResponse();
		}

		private void ClearSpells()
		{
			foreach (var c in ServerGame.Board.Cards)
			{
				if (c == null) continue;

				foreach (string s in c.SpellSubtypes)
				{
					switch (s)
					{
						case CardBase.SimpleSubtype:
							c.Discard();
							break;
						case CardBase.DelayedSubtype:
						case CardBase.VanishingSubtype:
							if (c.TurnsOnBoard >= c.Duration)
							{
								TriggeringEventContext context = new(game: ServerGame, CardBefore: c);
								c.Discard();
								context.CacheCardInfoAfter();
								TriggerForCondition(Trigger.Vanish, context);
							}
							break;
					}
				}
			}
		}

		public async Task ResolveNextStackEntry()
		{
			var (stackable, context) = stack.Pop();
			if (stackable == null) await StackEmptied();
			else
			{
				//GD.Print($"Resolving next stack entry: {stackable}, {context}");
				//inform the players that they no longer can respond, in case they were somehow still thinking they could
				foreach (var p in ServerGame.serverPlayers) ServerGame.Notifier.RequestNoResponse(p);

				//set the current stack entry to the appropriate value. this is used to check if something is currently resolving.
				CurrStackEntry = stackable;

				//actually resolve the thing
				await stackable.StartResolution(context);

				//after it resolves, tell the clients it's done resolving
				ServerGame.Notifier.RemoveStackEntry(currStackIndex);
				//take note that nothing is resolving
				CurrStackEntry = null;
				//and see if there's antyhing to resolve next.
				await CheckForResponse();
			}
		}

		private void RemoveHangingEffect(HangingEffect hangingEff)
		{
			hangingEffectMap[hangingEff.endCondition].Remove(hangingEff);
			//Not all hanging effects can fall off
			if (!string.IsNullOrEmpty(hangingEff.fallOffCondition))
				hangingEffectFallOffMap[hangingEff.fallOffCondition].Remove(hangingEff);
		}

		/// <summary>
		/// Cancel any stack entries <paramref name="eff"/> had on the stack, and any hanging effects caused by <paramref name="eff"/>
		/// </summary>
		/// <param name="eff"></param>
		public void Cancel(Effect eff)
		{
			//Remove effect from the stack, going top to bottom
			for (int i = stack.Count - 1; i >= 0; i--)
			{
				if (stack.StackEntries.ElementAt(i) == eff)
				{
					stack.Cancel(i);
					ServerGame.Notifier.RemoveStackEntry(i - 1);
				}
			}
			//Remove effect from hanging/delayed
			foreach (var triggerCondition in Trigger.TriggerConditions.Where(hangingEffectMap.ContainsKey))
			{
				foreach (var hangingEff in hangingEffectMap[triggerCondition].ToArray())
				{
					if (hangingEff.sourceEff == eff) RemoveHangingEffect(hangingEff);
				}
			}
		}
		#endregion the stack

		/// <summary>
		/// Accounts for optional triggers and ordering triggers, then pushes appropriate triggers to the stack.
		/// </summary>
		/// <param name="turnPlayer">The turn player, whose effects get pushed to the stack first.</param>
		private async Task CheckTriggers(ServerPlayer turnPlayer)
		{
			//get the list of triggers, and see if they're all still valid
			var triggered = triggeredTriggers.Dequeue();
			var stillValid = triggered.triggers.Where(t => t.StillValidForContext(triggered.context));

			//if there's no triggers, skip all this logic
			if (!stillValid.Any())
			{
				GD.Print($"All the triggers that were valid from {string.Join(",", triggered.triggers)} aren't anymore");
				return;
			}

			//if any triggers have not been responded to, make them get responded to.
			//this is saved so that we know what trigger to okay or not if it's responded
			currentlyCheckingOptionals = true;
			foreach (var t in stillValid)
			{
				//TODO this doesn't stop any subsequent calls to CheckTriggers
				if (!t.Responded) await t.Ask(t.Effect.OwningPlayer, triggered.context);
			}
			currentlyCheckingOptionals = false;

			//now that all optional triggers have been answered, time to deal with ordering.
			//if a player only has one trigger, don't bother asking them for an order.
			foreach (var p in ServerGame.Players)
			{
				var thisPlayers = stillValid.Where(t => t.serverEffect.OwningPlayer == p && t.Confirmed);
				if (thisPlayers.Count() == 1) thisPlayers.First().Order = 1;
			}

			//now, if there's any triggers that have been confirmed but not ordered (that is, more than one confirmed trigger),
			//then get an ordering from the player in question.
			var confirmed = stillValid.Where(t => t.Confirmed);
			if (!confirmed.All(t => t.Ordered))
			{
				//create a list to hold the tasks, so you can get trigger orderings from both players at once.
				List<Task> triggerOrderings = new();
				foreach (var p in ServerGame.serverPlayers)
				{
					var thisPlayers = confirmed.Where(t => t.serverEffect.OwningPlayer == p);
					if (thisPlayers.Any(t => !t.Ordered)) triggerOrderings.Add(p.awaiter.GetTriggerOrder(p, thisPlayers));
				}
				await Task.WhenAll(triggerOrderings);
			}

			//finally, push the triggers to the stack, in the proscribed order, starting with the turn player's
			foreach (var t in confirmed.Where(t => t.serverEffect.OwningPlayer == turnPlayer).OrderBy(t => t.Order))
				PushToStack(t.serverEffect, t.serverEffect.OwningServerPlayer, triggered.context);
			foreach (var t in confirmed.Where(t => t.serverEffect.OwningPlayer == turnPlayer.Enemy).OrderBy(t => t.Order))
				PushToStack(t.serverEffect, t.serverEffect.OwningServerPlayer, triggered.context);
		}

		/// <summary>
		/// Checks all triggers to see if any need to be addressed before stack resolution can continue.
		/// </summary>
		/// <param name="turnPlayer">The current turn player, who gets the first chance to accept or decline their triggers.</param>
		/// <returns></returns>
		private async Task CheckAllTriggers(ServerPlayer turnPlayer)
		{
			//note: you cannot use .Any(t => CheckTriggers(t)) because the collection would be modified while iterating
			//instead, just .Any() checks the queue after each time it's modified
			while (triggeredTriggers.Any())
			{
				await CheckTriggers(turnPlayer: turnPlayer);
				foreach (var tList in triggerMap.Values)
				{
					foreach (var t in tList) t.ResetConfirmation();
				}
			}
		}

		public async Task CheckForResponse()
		{
			//if we're already checking for response, don't check again.
			//checking again could cause us to consider the same set of triggers twice,
			//then dequeue twice, which would not consider that set of triggers.
			if (currentlyCheckingResponses || CurrStackEntry != null)
			{
				GD.Print($"Checked response while currently checking for response {currentlyCheckingResponses} " +
					$"or curr stack entry not null {CurrStackEntry != null}");
				return;
			}
			currentlyCheckingResponses = true;

			await CheckAllTriggers(ServerGame.TurnServerPlayer);

			//ResolveNextStackEntry eventually calls CheckForResponse again.
			//turn the flag off so that we can reenter CheckForResponse by the time that happens.
			currentlyCheckingResponses = false;
			await ResolveNextStackEntry();
		}

		private void ResolveHangingEffects(string condition, TriggeringEventContext context)
		{
			if (hangingEffectMap.ContainsKey(condition))
			{
				foreach (var toEnd in hangingEffectMap[condition]
					.Where(toEnd => toEnd.ShouldResolve(context))
					.ToArray())
				{
					if (toEnd.RemoveIfEnd) RemoveHangingEffect(toEnd);
					toEnd.Resolve(context);
				}
			}

			if (hangingEffectFallOffMap.ContainsKey(condition))
			{
				foreach (var toRemove in hangingEffectFallOffMap[condition]
					.Where(toRemove => toRemove.ShouldBeCanceled(context))
					.ToArray())
				{
					RemoveHangingEffect(toRemove);
				}
			}
		}

		public void TriggerForCondition(string condition, params TriggeringEventContext[] contexts)
		{
			foreach (var c in contexts) TriggerForCondition(condition, c);
		}

		public void TriggerForCondition(string condition, TriggeringEventContext context)
		{
			if (!ServerGame.GameHasStarted) return;

			GD.Print($"Triggering for condition {condition}, context {context}");
			//first resolve any hanging effects
			ResolveHangingEffects(condition, context);

			if (triggerMap.ContainsKey(condition))
			{
				/* Needs to be toArray()ed because cards might move out of correct state after this moment.
				 * Later, when triggers are being ordered, stuff like 1/turn will be rechecked. */
				var validTriggers = triggerMap[condition]
					.Where(t => t.ValidForTriggeringContext(context))
					.ToArray();
				if (!validTriggers.Any()) return;
				var triggers = new TriggersTriggered(triggers: validTriggers, context: context);
				GD.Print($"Triggers triggered: {string.Join(", ", triggers.triggers.Select(t => t.Card.ID + t.Blurb))}");
				triggeredTriggers.Enqueue(triggers);
			}

			if (condition != Trigger.Anything) TriggerForCondition(Trigger.Anything, context);
		}

		#region register to trigger condition
		public void RegisterTrigger(string condition, ServerTrigger trigger)
		{
			GD.Print($"Registering a new trigger from card {trigger.serverEffect.Card.CardName} to condition {condition}");
			if (!triggerMap.ContainsKey(condition))
				triggerMap.Add(condition, new List<ServerTrigger>());

			triggerMap[condition].Add(trigger);
		}

		public void RegisterHangingEffect(string condition, HangingEffect hangingEff, string fallOffCondition = default)
		{
			GD.Print($"Registering a new hanging effect to condition {condition}");
			if (!hangingEffectMap.ContainsKey(condition))
				hangingEffectMap.Add(condition, new List<HangingEffect>());

			hangingEffectMap[condition].Add(hangingEff);

			if (fallOffCondition != default) RegisterHangingEffectFallOff(fallOffCondition, hangingEff);
		}

		private void RegisterHangingEffectFallOff(string condition, HangingEffect hangingEff)
		{
			GD.Print($"Registering a new hanging effect to condition {condition}");
			if (!hangingEffectFallOffMap.ContainsKey(condition))
				hangingEffectFallOffMap.Add(condition, new List<HangingEffect>());

			hangingEffectFallOffMap[condition].Add(hangingEff);
		}
		#endregion register to trigger condition
	}
}