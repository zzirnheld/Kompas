using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Controllers
{
	public class DirectCardController<CardType, DisplayerType>
		where CardType : CardBase
		where DisplayerType : ICardInfoDisplayer
	{
		private CardViewBase<CardType, DisplayerType> view;
		private CardType card;

		public DirectCardController(CardViewBase<CardType, DisplayerType> view, CardType card)
		{
			this.view = view;
			this.card = card;
		}

		public void Display() => view.Show(card);
	}
}