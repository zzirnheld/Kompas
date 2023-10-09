using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

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

		protected override void PerformAdd(GameCard card, int? index, IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: game, CardBefore: card, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			base.PerformAdd(card, index, stackSrc);
			
			context.CacheCardInfoAfter();
			game.serverStackController.TriggerForCondition(Trigger.Annhilate, context);
			game.Notifier.NotifyAnnhilate(Owner, card, wasKnown);
		}
	}
}