using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

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

		public override void DisplayCardNumericStats(ICard card)
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

		public override void DisplayCardRulesText(ICard card)
		{
			_ = CardName ?? throw new System.NullReferenceException("Failed to init");
			_ = Subtypes ?? throw new System.NullReferenceException("Failed to init");

			CardName.Text = card.CardName;
			Subtypes.Text = card.SubtypeText;
		}
	}
}