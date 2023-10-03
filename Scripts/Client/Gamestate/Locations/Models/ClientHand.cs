using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientHand : Hand
	{
		public ClientHand(IPlayer owner, HandController handController) : base(owner, handController)
		{
		}
	}
}