using System;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views
{
	public class ClientTopLeftCardView : FocusableCardViewBase<ClientGameCard, ControlInfoDisplayer>
	{
		public ClientTopLeftCardView(ControlInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
		
		public void Select(ClientGameCard card) => base.Focus(card);
		public void Hover(ClientGameCard card, bool refresh = false) => base.Show(card, refresh);

		protected override void Show(ClientGameCard? card, bool refresh = false)
		{
			base.Show(card, refresh);

			if (card == null) return;
			card.CardController.AnythingRefreshed += (_, card) => Refresh(card);
		}

		/// <summary>
		/// If <paramref name="card"/> is the shown card, refreshes its shown information
		/// </summary>
		private void Refresh(GameCard? card)
		{
			if (card == ShownCard && card != null) Refresh();
		}
	}
}