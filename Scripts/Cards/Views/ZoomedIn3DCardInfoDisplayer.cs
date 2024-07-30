using Godot;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;
using Kompas.UI.TextBehavior;

namespace Kompas.Cards.Views
{
	public partial class ZoomedIn3DCardInfoDisplayer : MeshCardInfoDisplayerBase, ICardInfoDisplayer
	{
		[Export]
		private Label3D? N { get; set; }
		[Export]
		private Label3D? E { get; set; }
		[Export]
		private Label3D? Cost { get; set; }
		[Export]
		private Label3D? W { get; set; }

		[Export]
		private Label3D? CardName { get; set; }
		[Export]
		private Label3D? Subtypes { get; set; }
		[Export]
		private ShrinkRichTextOnOverrun? _effText;
		private ShrinkRichTextOnOverrun EffText => _effText
			?? throw new UnassignedReferenceException(nameof(_effText), this);

		public override void DisplayCardNumericStats(CardBase card)
		{
			_ = N ?? throw new System.NullReferenceException("Failed to init");
			_ = E ?? throw new System.NullReferenceException("Failed to init");
			_ = Cost ?? throw new System.NullReferenceException("Failed to init");
			_ = W ?? throw new System.NullReferenceException("Failed to init");

			N.Text = $"{card.N}";
			E.Text = $"{card.E}";
			Cost.Text = $"{card.Cost}";
			W.Text = $"{card.W}";
		}

		public override void DisplayCardRulesText(CardBase card)
		{
			_ = CardName ?? throw new System.NullReferenceException("Failed to init");
			_ = Subtypes ?? throw new System.NullReferenceException("Failed to init");

			CardName.Text = card.CardName;
			Subtypes.Text = card.SubtypeText;
			EffText.Text = card.BBCodeEffText;
		}
	}
}