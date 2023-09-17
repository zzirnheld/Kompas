using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	public class DeckBuilderCardView : FocusableCardViewBase<DeckBuilderCard, ICardInfoDisplayer>
	{
		public DeckBuilderCardView(ICardInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}