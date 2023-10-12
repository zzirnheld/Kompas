using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using System;

namespace Kompas.Cards.Views
{
	public abstract partial class MeshCardInfoDisplayerBase : Node3D, ICardInfoDisplayer
	{
		//Should be set to the materials
		[Export]
		private BaseMaterial3D FriendlyCardFrameMaterial { get; set; }
		[Export]
		private BaseMaterial3D EnemyCardFrameMaterial { get; set; }

		[Export]
		private BaseMaterial3D CardImageMaterial { get; set; }

		[Export]
		private MeshInstance3D[] FrameObjects { get; set; }

		public bool ShowingInfo { set => Visible = value; }

		public void DisplayCardImage(CardBase card)
		{
			CardImageMaterial.AlbedoTexture = card.CardFaceImage;
		}

		/* Testing */
		public override void _Ready()
		{
			base._Ready();
			DisplayCardController(true);
		} //*/

		public void DisplayCardController(bool friendly)
		{
			var material = friendly
				? FriendlyCardFrameMaterial
				: EnemyCardFrameMaterial;
			foreach (var obj in FrameObjects) obj.MaterialOverride = material;
		}

		public abstract void DisplayCardNumericStats(CardBase card);
		public abstract void DisplayCardRulesText(CardBase card);
	}
}