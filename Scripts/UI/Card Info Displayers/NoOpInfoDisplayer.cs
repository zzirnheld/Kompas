using Kompas.Cards.Models;

namespace Kompas.UI.CardInfoDisplayers
{
	public class NoOpInfoDisplayer : ICardInfoDisplayer
	{
		public bool ShowingInfo { set { } }
		public void DisplayCardImage(ICard card) { }
		public void DisplayCardNumericStats(ICard card) { }
		public void DisplayCardRulesText(ICard card) { }
	}
}