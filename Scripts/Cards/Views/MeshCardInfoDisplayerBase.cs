using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	public abstract partial class MeshCardInfoDisplayerBase : Node3D, IHoverableCardInfoDisplayer
	{
		/// <summary>
		/// Name in the frame shader of the "albedo" variable,
		/// because Godot requires that you set these things by strings.
		/// Like an animal.
		///</summary>
		private const string ShaderAlbedoUniformName = "Albedo";

		private const string ShaderGreyscaleUniformName = "Greyscale";

		private const string ShaderTextureUniformName = "Texture";

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

		public IEnumerable<VisualInstance3D> AllVisibleObjects => this.GetDescendants()
			.OfType<VisualInstance3D>();

		public event EventHandler<string>? BeginHoverKeyword;
		public event EventHandler<string>? EndHoverKeyword;

		protected void BeginHover(string keyword) => BeginHoverKeyword?.Invoke(this, keyword);
		protected void EndHover(string keyword) => EndHoverKeyword?.Invoke(this, keyword);

		private MeshInstance3D[] CardImageObjects => _cardImageObjects
			?? throw new UnassignedReferenceException(nameof(_cardImageObjects));

		public bool ShowingInfo { set => Visible = value; }

		public virtual void DisplayCardImage(CardBase card)
		{
			_ = CardImageObjects ?? throw new System.NullReferenceException("Failed to init");
			if (card.CardFaceImage == null)
			{
				Logger.Err($"Null card image for {card.CardName}! You haven't figured out yet what to do about that!");
				return;
			}

			foreach (var obj in CardImageObjects)
			{
				if (obj.MaterialOverride is not ShaderMaterial mat)
					throw new System.InvalidOperationException($"{obj}'s material is not a ShaderMaterial, can't set its albedo texture");

				mat.SetShaderParameter(ShaderTextureUniformName, card.CardFaceImage);
			}
		}

		/* Testing - add * / here to use to test color.
		public override void _Ready()
		{
			base._Ready();
			DisplayFrame(true);
		} //*/

		public void DisplayFrame(Color color)
		{
			_ = FrameObjects ?? throw new System.NullReferenceException("Failed to init");

			foreach (var obj in FrameObjects)
			{
				obj.MaterialOverride = CardFrameMaterial;
				obj.SetInstanceShaderParameter(ShaderAlbedoUniformName, color);
			}
		}

		public void DisplayGreyedOut(bool greyedOut)
		{
			foreach (var obj in FrameObjects)
			{
				obj.SetInstanceShaderParameter(ShaderGreyscaleUniformName, greyedOut);
			}
			foreach (var obj in CardImageObjects)
			{
				obj.SetInstanceShaderParameter(ShaderGreyscaleUniformName, greyedOut);
			}
		}

		public abstract void DisplayCardNumericStats(CardBase card);
		public abstract void DisplayCardRulesText(CardBase card);
		public void DisplayUnselectedValidTarget(bool validTarget) { }
		public void DisplayCurrentTarget(bool currentTarget) { }
		public void DisplayEffectSource(bool effectSource) { }

		public void UpdateZoomedInLayerMask(uint layerMask) { }
	}
}