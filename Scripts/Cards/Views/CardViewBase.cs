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
	public abstract class CardViewBase<TCard, DisTPlayer>
		where TCard : class, ICard
		where DisTPlayer : ICardInfoDisplayer
	{
		/// <summary>
		/// The card currently being shown to the user.
		/// </summary>
		public TCard? ShownCard { get; private set; }

		public DisTPlayer InfoDisplayer { get; }

		public class CardChange
		{
			public TCard? Old { get; init; }
			public TCard? New { get; init; }
		}

		public event EventHandler<CardChange>? ChangeShownCard;

		protected CardViewBase(DisTPlayer infoDisplayer)
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
		protected virtual void Show(TCard? card, bool refresh = false)
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

		protected virtual void Display(TCard shownCard)
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