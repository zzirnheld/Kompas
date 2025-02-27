using System;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views;

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
	public CardType? FocusedCard { get; private set; }

	public event EventHandler<CardChange>? FocusChange;

	protected FocusableCardViewBase(DisplayerType infoDisplayer)
		: base(infoDisplayer)
	{ }

	/// <summary>
	/// Focus on a given card,
	// showing it whenever we don't want to show something else
	// (something else like a temporary hover over)
	/// </summary>
	protected virtual void Focus(CardType? card)
	{
		ShiftFocus(card);
		Show(card);
	}

	/// <summary>
	/// Shifts tracked focus to the given card.
	/// If we're not currently doing something like hovering over another card,
	/// this is the one we should be showing.
	/// Doesn't show the card, though. See <see cref="Focus"/>
	/// </summary>
	protected void ShiftFocus(CardType? card)
	{
		var oldFocus = FocusedCard;
		FocusedCard = card;
		FocusChange?.Invoke(this, new CardChange() { Old = oldFocus, New = card });
	}

	protected override void Show(CardType? card, bool refresh = false)
	{
		base.Show(card ?? FocusedCard, refresh);
	}
}