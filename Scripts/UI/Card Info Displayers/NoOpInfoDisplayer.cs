using Kompas.Cards.Models;

namespace Kompas.UI.CardInformationDisplayers
{
	public class NoOpInfoDisplayer : ICardInfoDisplayer
	{
		public bool ShowingInfo { set { } }
		public void DisplayCardImage(CardBase card) { }
		public void DisplayCardNumericStats(CardBase card) { }
		public void DisplayCardRulesText(CardBase card) { }
	}
}