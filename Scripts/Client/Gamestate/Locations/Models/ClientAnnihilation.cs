using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientAnnihilation : Annihilation
	{
		private readonly ClientPlayer owner;
		public override Player Owner => owner;
	}
}