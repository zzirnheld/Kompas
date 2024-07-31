using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientHand : Hand
	{
		public ClientHand(IPlayer owner, HandController handController, HandController illusoryController)
			: base(owner, handController, illusoryController) { }

		public void IncrementHand()
		{
			//throw new System.NotImplementedException();
		}

		public void DecrementHand()
		{
			//throw new System.NotImplementedException();
		}
	}
}