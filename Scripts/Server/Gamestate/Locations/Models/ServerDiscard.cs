using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;

namespace Kompas.Server.Gamestate.Locations.Models
{
	public class ServerDiscard : Discard
	{
		private readonly ServerGame game;

		public ServerDiscard(IPlayer owner, DiscardController discardController, ServerGame game)
			: base(owner, discardController)
		{
			this.game = game;
		}

		protected override void PerformAdd(GameCard card, int? index, IStackable? stackSrc = null)
		{	
			bool wasKnown = card.KnownToEnemy;
			
			var contexts = IEventContext.Build(Trigger.Discard)
				.PrimarilyAffecting(card)
				.CausedBy(stackSrc)
				.ForPlayer(Owner)
				.Capture(() => base.PerformAdd(card, index, stackSrc));
			game.StackController.Trigger(contexts);

			Networking.ServerNotifier.NotifyDiscard(Owner, card, wasKnown);
		}
	}
}