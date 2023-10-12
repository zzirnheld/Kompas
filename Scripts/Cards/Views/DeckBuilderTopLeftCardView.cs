using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;

namespace Kompas.Cards.Views
{
	//TODO: this will change once i'm using something that isn't the placeholder card info displaying
	public class DeckBuilderTopLeftCardView : FocusableCardViewBase<DeckBuilderCard, ControlInfoDisplayer>
	{
		public DeckBuilderTopLeftCardView(ControlInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }

		public void Show(DeckBuilderCard card) => base.Show(card);
	}
}