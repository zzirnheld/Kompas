using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerAnnihilation : Annihilation
	{
		private readonly ServerGame game;

		public ServerAnnihilation(IPlayer owner, AnnihilationController annihilationController, ServerGame game)
			: base(owner, annihilationController)
		{
			this.game = game;
		}

		protected override void PerformAdd(GameCard card, int? index, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, cardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.PerformAdd(card, index, stackSrc);
			
			context.CacheCardInfoAfter();
			game.StackController.TriggerForCondition(Trigger.Annhilate, context);
			ServerNotifier.NotifyAnnhilate(Owner, card, wasKnown);
		}
	}
}