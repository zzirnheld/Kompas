using Kompas.Cards.Models;
using Kompas.UI.CardInformationDisplayers;

namespace Kompas.Cards.Views
{
	/// <summary>
	/// Defines the behavior for displaying card information, while not specifying the details of the implementation
	/// </summary>
	public abstract class FocusableCardViewBase<CardType, DisplayerType>
		: CardViewBase<CardType, DisplayerType>
		where CardType : CardBase
		where DisplayerType : ICardInfoDisplayer
	{
		/// <summary>
		/// The card being "focused" on.
		/// If we're not currently doing something like hovering over another card,
        /// this is the one we should be showing, as a fallback
		/// </summary>
		public CardType FocusedCard { get; private set; }

		protected FocusableCardViewBase(DisplayerType infoDisplayer)
			: base(infoDisplayer)
		{ }

		/// <summary>
		/// Focus on a given card.
		/// If we're not currently doing something like hovering over another card, this is the one we should be showing
		/// </summary>
		/// <param name="card"></param>
		public virtual void Focus(CardType card)
		{
			FocusedCard = card;
			Show(card);
		}

		public override void Show(CardType card, bool refresh = false)
		{
			base.Show(card ?? FocusedCard, refresh);
		}
	}
}