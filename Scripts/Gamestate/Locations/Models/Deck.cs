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
	{
		public IGameCard? Topdeck { get; }

		public void Shuffle();
	}

	public interface IDeck<TCard, TPlayer> : ILocationModel<TCard, TPlayer>, IDeck
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		public new TCard? Topdeck { get; }
		
		public void Add(TCard card, int? index = null, IStackable? stackableCause = null);

		public void PushTopdeck(TCard card, IStackable? stackSrc = null);

		public void PushBottomdeck(TCard card, IStackable? stackSrc = null);

		public void ShuffleIn(TCard card, IStackable? stackSrc = null);
	}

	public abstract class Deck<TCard, TPlayer> : OwnedLocationModel<TCard, TPlayer>, IDeck<TCard, TPlayer>
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		private readonly IList<TCard> deck = new List<TCard>();
		public override IEnumerable<TCard> Cards => deck;

		public override Location Location => Location.Deck;

		private readonly DeckController deckController;

		protected Deck(TPlayer owner, DeckController deckController) : base(owner)
		{
			this.deckController = deckController;
			deckController.DeckModel = this;
		}

		public override int IndexOf(TCard card) => deck.IndexOf(card);
		public int DeckSize => deck.Count;
		public TCard? Topdeck => deck.FirstOrDefault();
		IGameCard? IDeck.Topdeck => Topdeck;
		public IGameCard? Bottomdeck => deck.LastOrDefault();

		protected override bool AllowAlreadyHereWhenAdd => true;

		protected override void PerformAdd(TCard card, int? index, IStackable? stackableCause)
		{
			if (index.HasValue) deck.Insert(index.Value, card);
			else deck.Add(card);

			deckController.Refresh();
		}

		//adding and removing cards
		public virtual void PushTopdeck(TCard card, IStackable? stackSrc = null)
			=> Add(card, index: 0, stackableCause: stackSrc);

		public virtual void PushBottomdeck(TCard card, IStackable? stackSrc = null)
			=> Add(card, stackableCause: stackSrc);

		public virtual void ShuffleIn(TCard card, IStackable? stackSrc = null)
		{
			Add(card, stackableCause: stackSrc);
			Shuffle();
		}

		/// <summary>
		/// Random access remove from deck
		/// </summary>
		public override void Remove(TCard card)
		{
			if (!deck.Contains(card))
				throw new CardNotHereException(Location, card, $"Couldn't remove {card.CardName} from deck, it wasn't in deck!");

			deck.Remove(card);
			deckController.Refresh();
		}

		//misc

		public void Shuffle() => CollectionsHelper.ShuffleInPlace(deck);

		public void BottomdeckMany(IEnumerable<TCard> cards, IStackable? stackSrc = null)
		{
			var toShuffleInOrder = CollectionsHelper.Shuffle(cards.ToList());
			foreach (var card in toShuffleInOrder) PushBottomdeck(card, stackSrc);
		}

		public IReadOnlyCollection<TCard> CardsThatFitRestriction(IRestriction<IGameCardInfo> cardRestriction, ResolutionContext context)
		{
			var cards = new List<TCard>();
			foreach (var c in deck)
			{
				if (c != null && cardRestriction.IsValid(c, context))
					cards.Add(c);
			}
			return cards;
		}
	}
}