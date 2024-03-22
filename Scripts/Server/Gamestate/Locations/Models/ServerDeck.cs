using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerDeck : Deck<ServerGameCard, ServerPlayer>
	{
		private readonly ServerGame game;

		public ServerDeck(ServerPlayer owner, DeckController deckController, ServerGame game)
			: base(owner, deckController)
		{
			this.game = game;
		}

		protected override void PerformAdd(ServerGameCard card, int? index = null, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			base.PerformAdd(card, index, stackSrc);

			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.ToDeck, context);
			Networking.ServerNotifier.NotifyDeckCount(Owner, Cards.Count());
		}

		public override void PushBottomdeck(ServerGameCard card, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;

			base.PushBottomdeck(card, stackSrc);
			
			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.Bottomdeck, context);
			ServerNotifier.NotifyBottomdeck(Owner, card, wasKnown);
		}

		public override void PushTopdeck(ServerGameCard card, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.PushTopdeck(card, stackSrc);
			
			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.Topdeck, context);
			ServerNotifier.NotifyTopdeck(Owner, card, wasKnown);
		}

		public override void ShuffleIn(ServerGameCard card, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.ShuffleIn(card, stackSrc);
			
			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.Reshuffle, context);
			ServerNotifier.NotifyReshuffle(Owner, card, wasKnown);
		}

		public override void Remove(ServerGameCard card)
		{
			base.Remove(card);
			Networking.ServerNotifier.NotifyDeckCount(Owner, Cards.Count());
		}

		public override void TakeControlOf(ServerGameCard card) => card.ServerController = Owner;
	}
}