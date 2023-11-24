using Godot;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.UI.GameStart
{
	public partial class SelectDeckInfoDisplayer : TextureRect, ICardInfoDisplayer
	{
		[Export]
		private TextureRect? _image;
		private TextureRect Image => _image
			?? throw new UnassignedReferenceException();

		public bool ShowingInfo { set { } } //TODO: consider having a fallback for avatar? in case of Bad Info

		public void DisplayCardImage(CardBase card)
		{
			Image.Texture = card.CardFaceImage;
		}

		public void DisplayCardNumericStats(CardBase card) { }

		public void DisplayCardRulesText(CardBase card) { }
	}
}