using Godot;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	public abstract partial class MeshCardInfoDisplayerBase : Node3D, ICardInfoDisplayer
	{
		/// <summary>
		/// Card frame material.
		/// Should be modified by Settings in whatever Settings controller,
		/// since that should then affect all instances of the material (since the per-scene box isn't checked)
		/// </summary>
		[Export]
		private ShaderMaterial? _cardFrameMaterial;
		private ShaderMaterial CardFrameMaterial => _cardFrameMaterial
			?? throw new UnassignedReferenceException(nameof(_cardFrameMaterial));

		[Export]
		private MeshInstance3D[]? _frameObjects;
		private MeshInstance3D[] FrameObjects => _frameObjects
			?? throw new UnassignedReferenceException(nameof(_frameObjects));

		[Export]
		private MeshInstance3D[]? _cardImageObjects;
		private MeshInstance3D[] CardImageObjects => _cardImageObjects
			?? throw new UnassignedReferenceException(nameof(_cardImageObjects));

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

		/* Testing - add * / here to use to test color.
		public override void _Ready()
		{
			base._Ready();
			DisplayFrame(true);
		} //*/

		public void DisplayFrame(bool friendly, Vector4 color)
		{
			_ = FrameObjects ?? throw new System.NullReferenceException("Failed to init");

			foreach (var obj in FrameObjects)
			{
				obj.MaterialOverride = CardFrameMaterial;
				obj.SetInstanceShaderParameter("Albedo", color);
			}
		}

		public abstract void DisplayCardNumericStats(CardBase card);
		public abstract void DisplayCardRulesText(CardBase card);
		public void DisplayValidTarget(bool validTarget) { }
		public void DisplayCurrentTarget(bool currentTarget) { }
		public void DisplayEffectSource(bool effectSource) { }
	}
}