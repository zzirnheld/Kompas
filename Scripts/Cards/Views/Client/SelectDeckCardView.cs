using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.UI.GameStart;

namespace Kompas.Client.Cards.Views
{
	public class SelectDeckCardView : FocusableCardViewBase<CardBase, SelectDeckInfoDisplayer>
	{
		public SelectDeckCardView(SelectDeckInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}