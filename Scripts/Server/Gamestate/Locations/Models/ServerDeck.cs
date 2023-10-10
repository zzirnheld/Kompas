using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerDeck : Deck
	{
		private readonly ServerGame game;

		public ServerDeck(IPlayer owner, DeckController deckController, ServerGame game)
			: base(owner, deckController)
		{
			this.game = game;
		}

		protected override void PerformAdd(GameCard card, int? index = null, IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			base.PerformAdd(card, index, stackSrc);

			context.CacheCardInfoAfter();
			game.serverStackController.TriggerForCondition(Trigger.ToDeck, context);
			Networking.ServerNotifier.NotifyDeckCount(Owner, Cards.Count());
		}

		public override void PushBottomdeck(GameCard card, IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;

			base.PushBottomdeck(card, stackSrc);
			
			context.CacheCardInfoAfter();
			game.serverStackController.TriggerForCondition(Trigger.Bottomdeck, context);
			ServerNotifier.NotifyBottomdeck(Owner, card, wasKnown);
		}

		public override void PushTopdeck(GameCard card, IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.PushTopdeck(card, stackSrc);
			
			context.CacheCardInfoAfter();
			game.serverStackController.TriggerForCondition(Trigger.Topdeck, context);
			ServerNotifier.NotifyTopdeck(Owner, card, wasKnown);
		}

		public override void ShuffleIn(GameCard card, IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.ShuffleIn(card, stackSrc);
			
			context.CacheCardInfoAfter();
			game.serverStackController.TriggerForCondition(Trigger.Reshuffle, context);
			ServerNotifier.NotifyReshuffle(Owner, card, wasKnown);
		}

		public override void Remove(GameCard card)
		{
			base.Remove(card);
			Networking.ServerNotifier.NotifyDeckCount(Owner, Cards.Count());
		}
	}
}