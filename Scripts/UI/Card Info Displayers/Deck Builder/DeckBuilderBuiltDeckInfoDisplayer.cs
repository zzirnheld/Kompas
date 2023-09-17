using Godot;
using Kompas.Cards.Models;

namespace Kompas.UI.CardInfoDisplayers.DeckBuilder
{
	public partial class DeckBuilderBuiltDeckInfoDisplayer : TextureRect, ICardInfoDisplayer
	{
		public bool ShowingInfo { set => Visible = value; }

		public void DisplayCardImage(CardBase card)
		{
			GD.Print($"{Name} Displaying image of {card}. does it have an image? {null != card.CardFaceImage}");
			Texture = card.CardFaceImage;
		}

		public virtual void DisplayCardNumericStats(CardBase card) { }
		public virtual void DisplayCardRulesText(CardBase card) { }
	}
}