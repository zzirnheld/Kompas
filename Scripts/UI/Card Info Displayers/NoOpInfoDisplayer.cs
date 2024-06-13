using Kompas.Cards.Models;

namespace Kompas.UI.CardInfoDisplayers
{
	public class NoOpInfoDisplayer : ICardInfoDisplayer
	{
		public bool ShowingInfo { set { } }
		public void DisplayCardImage(CardBase card) { }
		public void DisplayCardNumericStats(CardBase card) { }
		public void DisplayCardRulesText(CardBase card) { }
		public void DisplayValidTarget(bool validTarget) { }
		public void DisplayCurrentTarget(bool currentTarget) { }
		public void DisplayEffectSource(bool effectSource) { }
	}
}