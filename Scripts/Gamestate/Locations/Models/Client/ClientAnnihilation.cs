using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models.Client
{
	public class ClientAnnihilation : Annihilation
	{
		public ClientAnnihilation(IPlayer owner, AnnihilationController annihilationController) : base(owner, annihilationController)
		{
		}
	}
}