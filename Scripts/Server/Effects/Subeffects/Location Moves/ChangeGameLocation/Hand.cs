using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Hand : ChangeGameLocation
	{
		protected override Location Destination => Location.Hand;

		protected override void ChangeLocation(GameCard card) => card.Hand(card.OwningPlayer, Effect);
	}
}