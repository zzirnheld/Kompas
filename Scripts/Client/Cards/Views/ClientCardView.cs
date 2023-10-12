using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;

namespace Kompas.Client.Cards.Views
{
	public class ClientCardView : FocusableCardViewBase<ClientGameCard, Zoomable3DCardInfoDisplayer>
	{
		public ClientCardView(Zoomable3DCardInfoDisplayer infoDisplayer, ClientGameCard card)
			: base(infoDisplayer)
		{
			Focus(card);
		}

		protected override void Display()
		{
			base.Display();

			InfoDisplayer.DisplayFrame(ShownCard.Friendly);
			InfoDisplayer.DisplayZoomed(zoomedIn: false); //For now, assume never zoomed in.
		}
	}
}