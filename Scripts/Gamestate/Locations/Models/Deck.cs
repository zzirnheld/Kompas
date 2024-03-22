using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;
using Kompas.Shared;

namespace Kompas.Gamestate.Locations.Models
{
	public interface IDeck : ILocationModel
	{ }

	public interface IDeck<CardType> : ILocationModel<CardType>, IDeck
		where CardType : GameCard
	{
		public void PushTopdeck(CardType card, IStackable? stackSrc = null);

		public void PushBottomdeck(CardType card, IStackable? stackSrc = null);

		public void ShuffleIn(CardType card, IStackable? stackSrc = null);
	}

	public abstract class Deck<CardType, PlayerType> : OwnedLocationModel<CardType, PlayerType>, IDeck<CardType>
		where CardType : GameCard
		where PlayerType : IPlayer
	{
		private readonly IList<CardType> deck = new List<CardType>();
		public override IEnumerable<CardType> Cards => deck;

		public override Location Location => Location.Deck;

		private readonly DeckController deckController;

		protected Deck(PlayerType owner, DeckController deckController) : base(owner)
		{
			this.deckController = deckController;
			deckController.DeckModel = this;
		}

		public override int IndexOf(CardType card) => deck.IndexOf(card);
		public int DeckSize => deck.Count;
		public GameCard? Topdeck => deck.FirstOrDefault();
		public GameCard? Bottomdeck => deck.LastOrDefault();

		protected override bool AllowAlreadyHereWhenAdd => true;

		protected override void PerformAdd(CardType card, int? index, IStackable? stackableCause)
		{
			if (index.HasValue) deck.Insert(index.Value, card);
			else deck.Add(card);

			deckController.Refresh();
		}

		//adding and removing cards
		public virtual void PushTopdeck(CardType card, IStackable? stackSrc = null)
			=> Add(card, index: 0, stackableCause: stackSrc);

		public virtual void PushBottomdeck(CardType card, IStackable? stackSrc = null)
			=> Add(card, stackableCause: stackSrc);

		public virtual void ShuffleIn(CardType card, IStackable? stackSrc = null)
		{
			Add(card, stackableCause: stackSrc);
			Shuffle();
		}

		/// <summary>
		/// Random access remove from deck
		/// </summary>
		public override void Remove(CardType card)
		{
			if (!deck.Contains(card))
				throw new CardNotHereException(Location, card, $"Couldn't remove {card.CardName} from deck, it wasn't in deck!");

			deck.Remove(card);
			deckController.Refresh();
		}

		//misc

		public void Shuffle() => CollectionsHelper.ShuffleInPlace(deck);

		public void BottomdeckMany(IEnumerable<CardType> cards, IStackable? stackSrc = null)
		{
			var toShuffleInOrder = CollectionsHelper.Shuffle(cards.ToList());
			foreach (var card in toShuffleInOrder) PushBottomdeck(card, stackSrc);
		}

		public IReadOnlyCollection<GameCard> CardsThatFitRestriction(IRestriction<IGameCardInfo> cardRestriction, ResolutionContext context)
		{
			var cards = new List<GameCard>();
			foreach (GameCard c in deck)
			{
				if (c != null && cardRestriction.IsValid(c, context))
					cards.Add(c);
			}
			return cards;
		}
	}
}