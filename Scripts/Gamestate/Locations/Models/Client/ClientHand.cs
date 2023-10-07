using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models.Client
{
	public class ClientHand : Hand
	{
		public ClientHand(IPlayer owner, HandController handController) : base(owner, handController) { }

		public void IncrementHand()
		{
			throw new System.NotImplementedException();
		}

		public void DecrementHand()
		{
			throw new System.NotImplementedException();
		}
	}
}