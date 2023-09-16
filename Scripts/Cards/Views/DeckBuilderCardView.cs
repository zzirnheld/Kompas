using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	public class DeckBuilderCardView : FocusableCardViewBase<DeckBuilderCard, ControlInfoDisplayer>
	{
		public DeckBuilderCardView(ControlInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}