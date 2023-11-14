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

		protected override void Show(ClientGameCard card, bool refresh = false)
		{
			base.Show(card, refresh);
			EventHandler handler = null;
			handler = (_, _) =>
			{
				card.CardController.Refreshed -= handler;
				Refresh();
			};
			card.CardController.Refreshed += handler;
		}
	}
}