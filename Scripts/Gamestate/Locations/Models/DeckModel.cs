using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate.Exceptions;
using Kompas.Shared;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class DeckModel : OwnedLocationModel
	{
		public override Location Location => Location.Deck;

		//rng for shuffling

		private readonly List<GameCard> deck = new List<GameCard>();
		public override IEnumerable<GameCard> Cards => deck;

		public override int IndexOf(GameCard card) => deck.IndexOf(card);
		public int DeckSize => deck.Count;
		public GameCard Topdeck => deck.FirstOrDefault();
		public GameCard Bottomdeck => deck.LastOrDefault();

		/// <summary>
		/// Sets the card's information to match this deck, but doesn't set its index.
		/// </summary>
		/// <param name="card">The card to add to this deck</param>
		/// <returns><see langword="true"/> if the add was completely successful.<br></br>
		/// <see langword="false"/> if the add failed in a way that isn't considered "impossible" (i.e. removing an avatar)</returns>
		protected virtual bool AddToDeck(GameCard card, int? index = null, IStackable stackSrc = null)
		{
			//Does not check if card is already in deck, because the functions to move around a card in deck are the same as those to add a card to deck
			SharedAddValidation(card, allowAlreadyHere: true);
			
			//Check if the card is successfully removed (if it's not, it's probably an avatar)
			if (card.Remove(stackSrc))
			{
				if (index.HasValue) deck.Insert(index.Value, card);
				else deck.Add(card);
				card.LocationModel = this;
				card.ControllingPlayer = Owner;
				card.Position = null;
				return true;
			}
			return false;
		}

		//adding and removing cards
		public virtual bool PushTopdeck(GameCard card, IStackable stackSrc = null)
			=> AddToDeck(card, index: 0, stackSrc: stackSrc);

		public virtual bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
			=> AddToDeck(card, stackSrc: stackSrc);

		public virtual bool ShuffleIn(GameCard card, IStackable stackSrc = null)
		{
			bool ret = AddToDeck(card, stackSrc: stackSrc);
			if (ret) Shuffle();
			return ret;
		}

		/// <summary>
		/// Random access remove from deck
		/// </summary>
		public override void Remove(GameCard card)
		{
			if (!deck.Contains(card))
				throw new CardNotHereException(Location, card, $"Couldn't remove {card.CardName} from deck, it wasn't in deck!");

			deck.Remove(card);
		}

		//misc

		public void Shuffle() => CollectionsHelper.ShuffleInPlace(deck);

		public void BottomdeckMany(IEnumerable<GameCard> cards, IStackable stackSrc = null)
		{
			var toShuffleInOrder = CollectionsHelper.Shuffle(cards.ToList());
			foreach (var card in toShuffleInOrder) PushBottomdeck(card, stackSrc);
		}

		public List<GameCard> CardsThatFitRestriction(IRestriction<GameCardBase> cardRestriction, ResolutionContext context)
		{
			List<GameCard> cards = new List<GameCard>();
			foreach (GameCard c in deck)
			{
				if (c != null && cardRestriction.IsValid(c, context))
					cards.Add(c);
			}
			return cards;
		}
	}
}