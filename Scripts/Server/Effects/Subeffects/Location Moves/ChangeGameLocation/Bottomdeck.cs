using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Bottomdeck : ChangeGameLocation
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext) == null;
		protected override Location Destination => Location.Deck;

		protected override void ChangeLocation(GameCard card) => card.Bottomdeck(card.OwningPlayer, Effect);
	}
}