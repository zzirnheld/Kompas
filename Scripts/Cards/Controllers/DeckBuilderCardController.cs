using Kompas.Cards.Models;
using Kompas.Cards.Views;

namespace Kompas.Cards.Controllers
{
	public class DeckBuilderCardController
	{
		private DeckBuilderCardView view;
		private DeckBuilderCard card;

		public DeckBuilderCardController(DeckBuilderCardView view, DeckBuilderCard card)
		{
			this.view = view;
			this.card = card;
		}

		public void Display() => view.Show(card);
	}
}