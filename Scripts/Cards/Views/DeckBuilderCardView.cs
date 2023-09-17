using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;

namespace Kompas.Cards.Views
{
	public class DeckBuilderCardView : FocusableCardViewBase<DeckBuilderCard, DeckBuilderInfoDisplayer>
	{
		public DeckBuilderCardView(DeckBuilderInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}