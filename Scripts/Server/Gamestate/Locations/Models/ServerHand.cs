using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerHand : Hand
	{
		private readonly ServerGame game;

		public ServerHand(IPlayer owner, HandController handController, ServerGame game)
			: base(owner, handController)
		{
			this.game = game;
		}

		protected override void PerformAdd(GameCard card, int? index, IStackable? stackSrc = null)
		{
			//var context = new TriggeringEventContext(game: game, cardBefore: card, stackableCause: stackSrc, player: Owner);
			var builder = TriggeringEventContext.BuildContext(game)
				.PrimarilyAffecting(card)
				.CausedBy(stackSrc)
				.Affecting(Owner);
			bool wasKnown = card.KnownToEnemy;

			base.PerformAdd(card, index, stackSrc);

			var context = builder.CacheToFinalize();
			game.StackController.TriggerForCondition(Trigger.Rehand, context);
			Networking.ServerNotifier.NotifyRehand(Owner, card, wasKnown);
		}
	}
}