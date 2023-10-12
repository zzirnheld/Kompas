using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using System;

namespace Kompas.Cards.Views
{
	public partial class ZoomedOut3DCardInfoDisplayer : MeshCardInfoDisplayerBase, ICardInfoDisplayer
	{
		//Text is a noop
		public override void DisplayCardNumericStats(CardBase card) { }
		public override void DisplayCardRulesText(CardBase card) { }
	}
}