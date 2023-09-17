using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;

namespace Kompas.Cards.Views
{
	public class DeckBuilderCardView : FocusableCardViewBase<DeckBuilderCard, DeckBuilderBuiltDeckInfoDisplayer>
	{
		public DeckBuilderCardView(DeckBuilderBuiltDeckInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}