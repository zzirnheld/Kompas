using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
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
			bool wasKnown = card.KnownToEnemy;
			
			var contexts = IEventContext.Build(Trigger.Annhilate)
				.PrimarilyAffecting(card)
				.CausedBy(stackSrc)
				.ForPlayer(Owner)
				.Capture(() => base.PerformAdd(card, index, stackSrc));
			game.StackController.Trigger(contexts);

			ServerNotifier.NotifyAnnhilate(Owner, card, wasKnown);
		}
	}
}