using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.UI.Client.GameStart;

namespace Kompas.Cards.Views.Client
{
	public class SelectDeckCardView : FocusableCardViewBase<CardBase, SelectDeckInfoDisplayer>
	{
		public SelectDeckCardView(SelectDeckInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }
	}
}