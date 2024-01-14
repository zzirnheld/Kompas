using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

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
		private BaseMaterial3D? FriendlyCardFrameMaterial { get; set; }
		/// <summary>
		/// Enemy card frame material.
		/// Defaults to shiny grey.
		/// Should be modified by Settings in whatever Settings controller,
		/// since that should then affect all instances of the material (since the per-scene box isn't checked)
		/// </summary>
		[Export]
		private BaseMaterial3D? EnemyCardFrameMaterial { get; set; }

		[Export]
		private MeshInstance3D[]? FrameObjects { get; set; }

		[Export]
		private MeshInstance3D[]? CardImageObjects { get; set; }

		public bool ShowingInfo { set => Visible = value; }

		public virtual void DisplayCardImage(CardBase card)
		{
			_ = CardImageObjects ?? throw new System.NullReferenceException("Failed to init");

			foreach (var obj in CardImageObjects)
			{
				if (obj.MaterialOverride is not BaseMaterial3D mat)
					throw new System.InvalidOperationException($"{obj}'s material is not a BaseMaterial3D, can't set its albedo texture");

				mat.AlbedoTexture = card.CardFaceImage;
			}
		}

		/* Testing */
		public override void _Ready()
		{
			base._Ready();
			DisplayFrame(true);
		} //*/

		public void DisplayFrame(bool friendly)
		{
			_ = FrameObjects ?? throw new System.NullReferenceException("Failed to init");

			var material = friendly
				? FriendlyCardFrameMaterial
				: EnemyCardFrameMaterial;
			foreach (var obj in FrameObjects) obj.MaterialOverride = material;
		}

		public abstract void DisplayCardNumericStats(CardBase card);
		public abstract void DisplayCardRulesText(CardBase card);
	}
}