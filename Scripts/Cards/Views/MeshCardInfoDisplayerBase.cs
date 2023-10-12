using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using System;

namespace Kompas.Cards.Views
{
	public abstract partial class MeshCardInfoDisplayerBase : Node3D, ICardInfoDisplayer
	{
		/// <summary>
        /// Friendly card frame material.
        /// Defaults to shiny gold.
        /// Should be modified by Settings in whatever Settings controller,
        /// since that should then affect all instances of the material (since the per-scene box isn't checked)
        /// </summary>
		[Export]
		private BaseMaterial3D FriendlyCardFrameMaterial { get; set; }
		/// <summary>
        /// Enemy card frame material.
        /// Defaults to shiny grey.
        /// Should be modified by Settings in whatever Settings controller,
        /// since that should then affect all instances of the material (since the per-scene box isn't checked)
        /// </summary>
		[Export]
		private BaseMaterial3D EnemyCardFrameMaterial { get; set; }

		[Export]
		private MeshInstance3D[] FrameObjects { get; set; }

		public bool ShowingInfo { set => Visible = value; }

		public virtual void DisplayCardImage(CardBase card)
		{
			//Image manipulation is done in the Zoomable3DCardInfoDisplayer, so we don't set the albedo texture twice.
			//Consider not implementing ICardInfoDisplayer since it doesn't really?
			throw new System.NotImplementedException("If you want to use the MeshCardInfoDisplayer this way, maybe consider splitting off a version that does actually modify the card image?");
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