using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientHand : Hand
	{
		private readonly ClientPlayer owner;
		public override Player Owner => owner;
	}
}