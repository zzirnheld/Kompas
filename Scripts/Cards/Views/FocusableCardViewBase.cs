using System;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	/// <summary>
	/// Defines the behavior for displaying card information, while not specifying the details of the implementation
	/// </summary>
	public abstract class FocusableCardViewBase<TCard, DisTPlayer>
		: CardViewBase<TCard, DisTPlayer>
		where TCard : CardBase
		where DisTPlayer : ICardInfoDisplayer
	{
		/// <summary>
		/// The card being "focused" on.
		/// If we're not currently doing something like hovering over another card,
		/// this is the one we should be showing, as a fallback
		/// </summary>
		public TCard? FocusedCard { get; private set; }

		public event EventHandler<CardChange>? FocusChange;

		protected FocusableCardViewBase(DisTPlayer infoDisplayer)
			: base(infoDisplayer)
		{ }

		/// <summary>
		/// Focus on a given card.
		/// If we're not currently doing something like hovering over another card, this is the one we should be showing
		/// </summary>
		/// <param name="card"></param>
		protected virtual void Focus(TCard? card)
		{
			var oldFocus = FocusedCard;
			FocusedCard = card;
			FocusChange?.Invoke(this, new CardChange() { Old = oldFocus, New = card });
			Show(card);
		}

		protected override void Show(TCard? card, bool refresh = false)
		{
			base.Show(card ?? FocusedCard, refresh);
		}
	}
}