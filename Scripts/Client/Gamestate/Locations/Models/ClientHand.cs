using Godot;
using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientHand : Hand<ClientGameCard, ClientPlayer>
	{
		public ClientHand(ClientPlayer owner, HandController handController) : base(owner, handController) { }

		public void IncrementHand()
		{
			//throw new System.NotImplementedException();
		}

		public void DecrementHand()
		{
			throw new System.NotImplementedException();
		}

		public override void TakeControlOf(ClientGameCard card)
		{
			card.ControllingPlayer = Owner;
		}
	}
}