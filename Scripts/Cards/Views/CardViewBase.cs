using System;
using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	/// <summary>
	/// Defines the behavior for displaying card information, while not specifying the details of the implementation.
	/// Most implementations will need to forward calls to a Control or Node3D that actually has references to the relevant fields
	/// </summary>
	public abstract class CardViewBase<CardType, DisplayerType>
		where CardType : class, ICard
		where DisplayerType : ICardInfoDisplayer
	{
		/// <summary>
		/// The card currently being shown to the user.
		/// </summary>
		public CardType? ShownCard { get; private set; }

		public DisplayerType InfoDisplayer { get; }

		public class CardChange
		{
			public CardType? Old { get; init; }
			public CardType? New { get; init; }
		}

		public event EventHandler<CardChange>? ChangeShownCard;

		protected CardViewBase(DisplayerType infoDisplayer)
		{
			InfoDisplayer = infoDisplayer;
		}

		/// <summary>
		/// Force an update to the currently shown card's information being displayed
		/// </summary>
		public void Refresh()
		{
			Show(ShownCard, true);
		}

		/// <summary>
		/// Request that information be shown that reflects the given card.
		/// </summary>
		/// <param name="card"></param>
		/// <param name="refresh"></param>
		protected virtual void Show(CardType? card, bool refresh = false)
		{
			//Unless explicitly refreshing card, if already showing that card, no-op.
			if (card == ShownCard && !refresh) return;

			var old = ShownCard;
			ShownCard = card;
			ChangeShownCard?.Invoke(this, new() { Old = old, New = card});

			//If we're now showing nothing, hide the window and be done
			if (ShownCard == null) DisplayNothing();
			else Display(ShownCard);
		}

		/// <summary>
		/// Makes everything display nothing/clear out anything it's showing
		/// </summary>
		protected virtual void DisplayNothing()
		{
			InfoDisplayer.ShowingInfo = false;
		}

		protected virtual void Display(CardType shownCard)
		{
			//If not showing nothing, make sure we're showing information
			InfoDisplayer.ShowingInfo = true;
			//and display any relevant information for the card
			InfoDisplayer.DisplayCardRulesText(shownCard);
			InfoDisplayer.DisplayCardNumericStats(shownCard);
			InfoDisplayer.DisplayCardImage(shownCard);
		}
	}
}