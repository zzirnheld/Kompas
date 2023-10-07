using Kompas.Cards.Models;

namespace Kompas.Cards.Models.Client
{
	public class SelectDeckCard : CardBase
	{

		public SelectDeckCard(CardStats stats,
			string subtext, string[] spellTypes,
			bool unique, int radius, int duration,
			char cardType, string cardName, string fileName,
			string effText, string subtypeText)
			: base(stats, subtext, spellTypes, unique, radius, duration, cardType, cardName, fileName, effText, subtypeText)
		{
		}
	}
}