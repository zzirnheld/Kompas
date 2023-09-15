using Godot;
using Kompas.Cards.Models;

namespace Kompas.UI.CardInformationDisplayers
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
		private Label S { get; set; }
		[Export]
		private Label W { get; set; }
		[Export]
		private Label C { get; set; }
		[Export]
		private Label A { get; set; }
		[Export]
		private Label CardName { get; set; }
		[Export]
		private Label Subtypes { get; set; }
		[Export]
		private RichTextLabel EffText { get; set; }

		public bool ShowingInfo { set => Visible = value; }

		public void DisplayCardImage(CardBase card)
		{
			Image.Texture = card.CardFaceImage;
		}

		public void DisplayCardNumericStats(CardBase card)
		{
			N.Text = $"{card.N}";
			E.Text = $"{card.E}";
			S.Text = $"{card.S}";
			W.Text = $"{card.W}";
			C.Text = $"{card.C}";
			A.Text = $"{card.A}";
		}

		public void DisplayCardRulesText(CardBase card)
		{
			CardName.Text = card.CardName;
			Subtypes.Text = card.SubtypeText;
			EffText.Text = card.EffText;
		}
	}
}