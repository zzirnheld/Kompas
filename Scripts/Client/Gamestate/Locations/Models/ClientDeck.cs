using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientDeck : Deck<ClientGameCard, ClientPlayer>
	{
		public ClientDeck(ClientPlayer owner, DeckController deckController) : base(owner, deckController)
		{
		}

		public override void TakeControlOf(ClientGameCard card)
		{
			card.ControllingPlayer = Owner;
		}
	}
}