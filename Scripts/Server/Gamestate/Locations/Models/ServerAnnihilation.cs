using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Server.Cards.Models;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerAnnihilation : Annihilation<ServerGameCard, ServerPlayer>
	{
		private readonly ServerGame game;

		public ServerAnnihilation(ServerPlayer owner, AnnihilationController annihilationController, ServerGame game)
			: base(owner, annihilationController)
		{
			this.game = game;
		}

		public override void TakeControlOf(ServerGameCard card) => card.ServerController = Owner;

		protected override void PerformAdd(ServerGameCard card, int? index, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.PerformAdd(card, index, stackSrc);
			
			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.Annhilate, context);
			ServerNotifier.NotifyAnnhilate(Owner, card, wasKnown);
		}
	}
}