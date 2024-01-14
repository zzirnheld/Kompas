using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Play : ChangeGameLocation
	{
		protected override Location Destination => Location.Board;

		protected override void ChangeLocation(GameCard card) => card.Play(SpaceTarget, PlayerTarget, Effect);
	}
}