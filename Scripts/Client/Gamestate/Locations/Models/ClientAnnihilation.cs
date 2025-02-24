using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models;

public class ClientAnnihilation : Annihilation
{
	public ClientAnnihilation(IPlayer owner, IAnnihilationController annihilationController) : base(owner, annihilationController)
	{
	}
}