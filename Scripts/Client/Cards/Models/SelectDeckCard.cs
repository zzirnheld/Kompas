using Kompas.Cards.Models;

namespace Kompas.Client.Cards.Models
{
	public class SelectDeckCard : CardBase
	{

		public SelectDeckCard(CardStats stats,
			string? subtext, string[] spellTypes,
			bool unique, int radius, int duration,
			char TCard, string? cardName, string? fileName,
			string? effText, string? subtypeText)
			: base(stats, subtext, spellTypes, unique, radius, duration, TCard, cardName, fileName, effText, subtypeText)
		{
		}
	}
}