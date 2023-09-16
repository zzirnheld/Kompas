using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInformationDisplayers;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderBuiltDeckInfoDisplayer : TextureRect, ICardInfoDisplayer
	{
		public bool ShowingInfo { set => Visible = value; }

		public void DisplayCardImage(CardBase card)
		{
			Texture = card.CardFaceImage;
		}

		public void DisplayCardNumericStats(CardBase card) { }
		public void DisplayCardRulesText(CardBase card) { }
	}
}