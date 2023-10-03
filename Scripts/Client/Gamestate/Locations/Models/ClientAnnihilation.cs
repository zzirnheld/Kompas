using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientAnnihilation : Annihilation
	{
		//TODO initialize all of these in constructors
		public override Player Owner { get; }
	}
}