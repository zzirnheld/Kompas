using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientDiscard : Discard<ClientGameCard, ClientPlayer>
	{
		public ClientDiscard(ClientPlayer owner, DiscardController discardController) : base(owner, discardController)
		{
		}

		public override void TakeControlOf(ClientGameCard card)
		{
			card.ControllingPlayer = Owner;
		}
	}
}