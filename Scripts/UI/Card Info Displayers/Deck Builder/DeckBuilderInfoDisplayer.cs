using Godot;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.CardInfoDisplayers.DeckBuilder
{
	public partial class DeckBuilderInfoDisplayer : Control, ICardInfoDisplayer
	{
		[Export]
		private Texture2D? _fallbackCardImageTexture;
		private Texture2D FallbackCardImageTexture => _fallbackCardImageTexture
			?? throw new UnassignedReferenceException();

		[Export]
		private TextureRect? _cardFaceImage;
		protected TextureRect CardFaceImage => _cardFaceImage
			?? throw new UnassignedReferenceException();
		public bool ShowingInfo { set => Visible = value; }

		public override void _Ready()
		{
			base._Ready();
			Resized += () => Logger.Log($"Resizing {Name}");
		}

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
