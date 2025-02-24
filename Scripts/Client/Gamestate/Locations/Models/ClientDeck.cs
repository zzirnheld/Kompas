using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models;

public class ClientDeck : Deck
{
	public ClientDeck(IPlayer owner, IDeckController deckController) : base(owner, deckController)
	{
	}
}