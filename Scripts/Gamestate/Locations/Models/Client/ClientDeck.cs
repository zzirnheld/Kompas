using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models.Client
{
	public class ClientDeck : Deck
	{
		public ClientDeck(IPlayer owner, DeckController deckController) : base(owner, deckController)
		{
		}
	}
}