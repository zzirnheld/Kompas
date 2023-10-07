using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.UI.Client.GameStart
{
	public partial class SelectDeckInfoDisplayer : TextureRect, ICardInfoDisplayer
	{
		[Export]
		private TextureRect Image { get; set; }

		public bool ShowingInfo { set { } } //TODO: consider having a fallback for avatar? in case of Bad Info

		public void DisplayCardImage(CardBase card)
		{
			Image.Texture = card.CardFaceImage;
		}

		public void DisplayCardNumericStats(CardBase card) { }

		public void DisplayCardRulesText(CardBase card) { }
	}
}