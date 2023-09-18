using Godot;
using Kompas.Cards.Models;

namespace Kompas.UI.CardInfoDisplayers.DeckBuilder
{
	public partial class DeckBuilderInfoDisplayer : Control, ICardInfoDisplayer
	{
		[Export]
		private Texture2D FallbackCardImageTexture { get; set; }

		[Export]
		protected TextureRect CardFaceImage { get; private set; }

		public bool ShowingInfo { set => Visible = value; }

		public void Clear()
		{
			CardFaceImage.Texture = FallbackCardImageTexture;
		}

		public void DisplayCardImage(CardBase card)
		{
			CardFaceImage.Texture = card?.CardFaceImage ?? FallbackCardImageTexture;
		}

		public virtual void DisplayCardNumericStats(CardBase card) { }
		public virtual void DisplayCardRulesText(CardBase card) { }
	}
}
