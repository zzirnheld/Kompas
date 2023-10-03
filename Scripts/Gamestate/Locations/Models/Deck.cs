using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate.Exceptions;
using Kompas.Shared;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Deck : OwnedLocationModel
	{
		public override Location Location => Location.Deck;

		//rng for shuffling

		private readonly List<GameCard> deck = new();
		public override IEnumerable<GameCard> Cards => deck;

		public override int IndexOf(GameCard card) => deck.IndexOf(card);
		public int DeckSize => deck.Count;
		public GameCard Topdeck => deck.FirstOrDefault();
		public GameCard Bottomdeck => deck.LastOrDefault();

		protected override bool AllowAlreadyHereWhenAdd => true;

		protected override void PerformAdd(GameCard card, int? index)
		{
			if (index.HasValue) deck.Insert(index.Value, card);
			else deck.Add(card);
		}

		//adding and removing cards
		public virtual void PushTopdeck(GameCard card, IStackable stackSrc = null)
			=> Add(card, index: 0, stackableCause: stackSrc);

		public virtual void PushBottomdeck(GameCard card, IStackable stackSrc = null)
			=> Add(card, stackableCause: stackSrc);

		public virtual void ShuffleIn(GameCard card, IStackable stackSrc = null)
		{
			Add(card, stackableCause: stackSrc);
			Shuffle();
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

		public List<GameCard> CardsThatFitRestriction(IRestriction<IGameCard> cardRestriction, ResolutionContext context)
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