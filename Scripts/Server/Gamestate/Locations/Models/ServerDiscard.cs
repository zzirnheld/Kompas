using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerDiscard : Discard<ServerGameCard, ServerPlayer>
	{
		private readonly ServerGame game;

		public ServerDiscard(ServerPlayer owner, DiscardController discardController, ServerGame game)
			: base(owner, discardController)
		{
			this.game = game;
		}

		public override void TakeControlOf(ServerGameCard card)
		{
			card.ServerController = Owner;
		}

		protected override void PerformAdd(ServerGameCard card, int? index, IStackable? stackSrc = null)
		{
			GameCard? cause = null;
			if (stackSrc is Effect eff) cause = eff.Card;
			else if (stackSrc is Attack atk)
			{
				if (atk.attacker == card) cause = atk.defender;
				else if (atk.defender == card) cause = atk.attacker;
				else if (atk.attacker == card.AugmentedCard) cause = atk.defender;
				else if (atk.defender == card.AugmentedCard) cause = atk.attacker;
				else throw new System.ArgumentException($"Why is {card} neither the attacker nor defender, nor augmenting them, " +
					$"in the attack {atk} that caused it to be discarded?");
			}
			var context = new TriggeringEventContext(game: game, CardBefore: card, secondaryCardBefore: cause, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			
			base.PerformAdd(card, index, stackSrc);
			
			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.Discard, context);
			Networking.ServerNotifier.NotifyDiscard(Owner, card, wasKnown);
		}
	}
}