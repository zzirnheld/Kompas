using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using System;

namespace Kompas.Cards.Views
{
	public partial class ZoomedOut3DCardInfoDisplayer : Node3D, ICardInfoDisplayer
	{
		[Export]
		private BaseMaterial3D CardImageMaterial { get; set; }

		public bool ShowingInfo { set { } } //Always active - TODO should this hide the card? possibly?

		public void DisplayCardImage(CardBase card)
		{
			CardImageMaterial.AlbedoTexture = card.CardFaceImage;
		}

		//Text is a noop
		public void DisplayCardNumericStats(CardBase card) { }
		public void DisplayCardRulesText(CardBase card) { }
	}
}