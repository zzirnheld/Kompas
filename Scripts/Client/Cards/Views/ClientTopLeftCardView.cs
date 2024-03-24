using System;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.UI;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views
{
	public class ClientTopLeftCardView : TopLeftCardViewBase<ClientGameCard>
	{
		protected override CardRepository CardRepository => ShownCard?.Game.CardRepository
			?? throw new InvalidOperationException("Can't access a card repository while not showing cards!");

		public ClientTopLeftCardView(ControlInfoDisplayer infoDisplayer, ReminderTextPopup reminderTextPopup)
			: base(infoDisplayer, reminderTextPopup)
		{ }
		
		public new void Focus(ClientGameCard? card) => base.Focus(card);
		public void Hover(ClientGameCard? card, bool refresh = false) => base.Show(card, refresh);

		protected override void Show(ClientGameCard? card, bool refresh = false)
		{
			//unsubscribe before resubscribing, because otherwise refresh subscribes, and this explodes. hard.
			if (ShownCard != null) ShownCard.CardController.AnythingRefreshed -= Refresh;

			base.Show(card, refresh);

			if (card == null) return;
			card.CardController.AnythingRefreshed += Refresh;
		}

		//This is its own function, not a lambda, so it can unsubscribe.
		private void Refresh(object? _, GameCard? card) => Refresh(card);

		/// <summary>
		/// If <paramref name="card"/> is the shown card, refreshes its shown information
		/// </summary>
		private void Refresh(GameCard? card)
		{
			if (card == ShownCard && card != null) Refresh();
		}
	}
}