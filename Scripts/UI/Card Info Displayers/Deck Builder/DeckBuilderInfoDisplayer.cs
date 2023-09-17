using Godot;
using Kompas.Cards.Models;

namespace Kompas.UI.CardInfoDisplayers.DeckBuilder
{
	public partial class DeckBuilderInfoDisplayer : TextureRect, ICardInfoDisplayer
	{
		[Export]
		private Texture2D FallbackCardImageTexture { get; set; }

		public bool ShowingInfo { set => Visible = value; }

		public void Clear()
		{
			Texture = FallbackCardImageTexture;
		}

		public void DisplayCardImage(CardBase card)
		{
			Texture = card?.CardFaceImage ?? FallbackCardImageTexture;
		}

		public virtual void DisplayCardNumericStats(CardBase card) { }
		public virtual void DisplayCardRulesText(CardBase card) { }
	}
}
