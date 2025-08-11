using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models;

public class ClientDeck : Deck
{
	public ClientDeck(IPlayer owner, IDeckController deckController) : base(owner, deckController)
	{ }

	// NOTE: if I can make refreshing the deck a cheap operation, this should be fine?
	// May also wanna consider making the deck stay sorted client-side.
	// Sort by ID to maintain player's chosen deck order?
	public override void PushBottomdeck(GameCard card, IStackable? stackSrc = null)
	{
		if (card.LocationModel == this)
		{
			Logger.Log("Card was already in the same deck. No-op on client");
			return;
		}
		base.PushBottomdeck(card, stackSrc);
	}

	public override void PushTopdeck(GameCard card, IStackable? stackSrc = null)
	{
		if (card.LocationModel == this)
		{
			Logger.Log("Card was already in the same deck. No-op on client");
			return;
		}
		base.PushTopdeck(card, stackSrc);
	}

	public override void ShuffleIn(GameCard card, IStackable? stackSrc = null)
	{
		if (card.LocationModel == this)
		{
			Logger.Log("Card was already in the same deck. No-op on client");
			return;
		}
		base.ShuffleIn(card, stackSrc);
	}
}