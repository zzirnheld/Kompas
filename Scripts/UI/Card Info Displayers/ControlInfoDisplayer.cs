using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.UI.TextBehavior;

namespace Kompas.UI.CardInfoDisplayers
{
	public partial class ControlInfoDisplayer : Control, ICardInfoDisplayer
	{
		[Export]
		private TextureRect Image { get; set; }
		[Export]
		private Label N { get; set; }
		[Export]
		private Label E { get; set; }
		[Export]
		private Label Cost { get; set; }
		[Export]
		private Label W { get; set; }
		[Export]
		private ShrinkOnOverrun CardName { get; set; }
		[Export]
		private ShrinkOnOverrun Subtypes { get; set; }
		[Export]
		private ShrinkRichTextOnOverrun EffText { get; set; }

		//Consider moving these to a subclass if I ever want to 
		[Export]
		private TextureRect FrameImage { get; set; }

		public bool ShowingInfo
		{
			set => Visible = value;
		}

		public void DisplayCardImage(CardBase card)
		{
			Image.Texture = card.CardFaceImage;
			FrameImage.Texture = card.CardType == 'C' ? CardRepository.CharCardFrameTexture : CardRepository.NoncharCardFrameTexture;
		}

		public void DisplayCardNumericStats(CardBase card)
		{
			N.Text = $"{card.N}";
			E.Text = $"{card.E}";
			Cost.Text = $"{card.Cost}";
			W.Text = $"{card.W}";
		}

		public void DisplayCardRulesText(CardBase card)
		{
			CardName.ShrinkableText = card.CardName;
			Subtypes.ShrinkableText = card.SubtypeText;
			EffText.ShrinkableText = card.EffText;
		}
	}
}