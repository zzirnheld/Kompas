using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Annihilate : ChangeGameLocation
	{
		protected override Location Destination => Location.Annihilation;

		protected override void ChangeLocation(GameCard card) => card.Annihilate(Effect);
	}
}