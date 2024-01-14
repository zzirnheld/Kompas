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

		protected override void Display(ClientGameCard shownCard)
		{
			base.Display(shownCard);

			InfoDisplayer.DisplayFrame(shownCard.OwningPlayer.Friendly);
			InfoDisplayer.DisplayZoomed(zoomedIn: false); //For now, assume never zoomed in.

			DisplayTargeting(shownCard);
		}

		private void DisplayTargeting(ClientGameCard shownCard)
		{
			var targetingController = shownCard.ClientGame.ClientGameController.TargetingController
				?? throw new System.NullReferenceException("Forgot to init");
			InfoDisplayer.DisplayValidTarget(targetingController.IsUnselectedValidTarget(shownCard));
			InfoDisplayer.DisplayCurrentTarget(targetingController.IsSelectedTarget(shownCard));
		}
	}
}