using Kompas.Cards.Models;
using Kompas.UI.CardInformationDisplayers;

namespace Kompas.Cards.Views
{
	public class DeckBuilderCardView : FocusableCardViewBase<DeckBuilderCard, ControlInfoDisplayer>
	{
		public DeckBuilderCardView(ControlInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}