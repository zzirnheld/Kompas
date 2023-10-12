using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using System;

namespace Kompas.Cards.Views
{
	public partial class ZoomedIn3DCardInfoDisplayer : MeshCardInfoDisplayerBase, ICardInfoDisplayer
	{
		[Export]
		private Label3D N { get; set; }
		[Export]
		private Label3D E { get; set; }
		[Export]
		private Label3D Cost { get; set; }
		[Export]
		private Label3D W { get; set; }

		[Export]
		private Label3D CardName { get; set; }
		[Export]
		private Label3D Subtypes { get; set; }

		public override void DisplayCardNumericStats(CardBase card)
		{
			N.Text = $"{card.N}";
			E.Text = $"{card.E}";
			Cost.Text = $"{card.Cost}";
			W.Text = $"{card.W}";
		}

		public override void DisplayCardRulesText(CardBase card)
		{
			CardName.Text = card.CardName;
			Subtypes.Text = card.SubtypeText;
		}
	}
}