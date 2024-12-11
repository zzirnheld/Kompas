using System;
using Godot;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views;

/// <summary>
/// currently mostly a placeholder/wrapper for multiple card info displayers. later, should account for stuff like:
/// Whether the camera is zoomed in or not
/// Whether the card is a char or not
/// TODO: should the material for the card image maybe be on this parent?
/// TODO also the in/out 3d info displayers maybe should inherit from a shared class that handles card frames
/// (that way each zoom level handles its own frame objects' material setting based on player owner)
/// </summary>
public partial class Zoomable3DCardInfoDisplayer : Node3D, IHoverableCardInfoDisplayer
{
	[Export]
	private MeshCardInfoDisplayerBase? _zoomedOut;
	private MeshCardInfoDisplayerBase ZoomedOut => _zoomedOut
		?? throw new UnassignedReferenceException(nameof(_zoomedOut), this);

	[Export]
	private MeshCardInfoDisplayerBase? _zoomedIn;
	private MeshCardInfoDisplayerBase ZoomedIn => _zoomedIn
		?? throw new UnassignedReferenceException(nameof(_zoomedIn), this);

	[Export]
	private BaseMaterial3D? _cardImageMaterial;
	private BaseMaterial3D CardImageMaterial => _cardImageMaterial
		?? throw new UnassignedReferenceException(nameof(_cardImageMaterial), this);

	[Export]
	private GpuParticles3D? _validTargetParticles;
	private GpuParticles3D ValidTargetParticles => _validTargetParticles
		?? throw new UnassignedReferenceException(nameof(_validTargetParticles), this);
	[Export]
	private GpuParticles3D? _currentTargetParticles;
	private GpuParticles3D CurrentTargetParticles => _currentTargetParticles
		?? throw new UnassignedReferenceException(nameof(_currentTargetParticles), this);
	[Export]
	private GpuParticles3D? _effectSourceParticles;

	public event EventHandler<string>? BeginHoverKeyword;
	public event EventHandler<string>? EndHoverKeyword;

	private GpuParticles3D EffectSourceParticles => _effectSourceParticles
		?? throw new UnassignedReferenceException(nameof(_effectSourceParticles), this);

	public override void _Ready()
	{
		base._Ready();
		ZoomedIn.BeginHoverKeyword += this.BeginHoverKeyword;
		ZoomedOut.BeginHoverKeyword += this.BeginHoverKeyword;

		ZoomedIn.EndHoverKeyword += this.EndHoverKeyword;
		ZoomedOut.EndHoverKeyword += this.EndHoverKeyword;
	}

	public bool ShowingInfo { set => Visible = value; }

	public void DisplayCardImage(CardBase card)
	{
		ZoomedOut.DisplayCardImage(card);
		ZoomedIn.DisplayCardImage(card);
	}

	public void DisplayCardNumericStats(CardBase card)
	{
		ZoomedOut.DisplayCardNumericStats(card);
		ZoomedIn.DisplayCardNumericStats(card);
	}

	public void DisplayCardRulesText(CardBase card)
	{
		ZoomedOut.DisplayCardRulesText(card);
		ZoomedIn.DisplayCardRulesText(card);
	}

	public void DisplayFrame(Color albedo)
	{
		ZoomedOut.DisplayFrame(albedo);
		ZoomedIn.DisplayFrame(albedo);
	}

	public void DisplayGreyedOut(bool greyedOut)
	{
		ZoomedOut.DisplayGreyedOut(greyedOut);
		ZoomedIn.DisplayGreyedOut(greyedOut);	
	}

	public void UpdateZoomedInLayerMask(uint layerMask)
	{
		//GD.Print($"Setting zoomed in descendants to {layerMask}");
		foreach (var obj in ZoomedIn.AllVisibleObjects)
		{
			//GD.Print($"setting {obj} to {layerMask}");
			obj.Layers = layerMask;
		}
	}

	public void DisplayUnselectedValidTarget(bool validTarget)
		=> ValidTargetParticles.Emitting = validTarget;

	public void DisplayCurrentTarget(bool currentTarget)
		=> CurrentTargetParticles.Emitting = currentTarget;

	public void DisplayEffectSource(bool effectSource)
		=> EffectSourceParticles.Emitting = effectSource;
}