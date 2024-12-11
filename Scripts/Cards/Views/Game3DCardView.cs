using Kompas.Cards.Models;

namespace Kompas.Cards.Views;

public partial class Game3DCardView : CardViewBase<GameCard, Zoomable3DCardInfoDisplayer>
{
	public Game3DCardView(Zoomable3DCardInfoDisplayer infoDisplayer) : base(infoDisplayer)
	{
	}
}