using Godot;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	/// <summary>
	/// currently mostly a placeholder/wrapper for multiple card info displayers. later, should account for stuff like:
	/// Whether the camera is zoomed in or not
	/// Whether the card is a char or not
	/// TODO: should the material for the card image maybe be on this parent?
	/// TODO also the in/out 3d info displayers maybe should inherit from a shared class that handles card frames
	/// (that way each zoom level handles its own frame objects' material setting based on player owner)
	/// </summary>
	public partial class Zoomable3DCardInfoDisplayer : Node3D, ICardInfoDisplayer
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
		private GpuParticles3D EffectSourceParticles => _effectSourceParticles
			?? throw new UnassignedReferenceException(nameof(_effectSourceParticles), this);

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

		//FUTURE: replace with enum?
		public void DisplayZoomed(bool zoomedIn)
		{
			ZoomedOut.ShowingInfo = !zoomedIn;
			ZoomedIn.ShowingInfo = zoomedIn;
		}

		public void DisplayUnselectedValidTarget(bool validTarget)
			=> ValidTargetParticles.Emitting = validTarget;

		public void DisplayCurrentTarget(bool currentTarget)
			=> CurrentTargetParticles.Emitting = currentTarget;

		public void DisplayEffectSource(bool effectSource)
			=> EffectSourceParticles.Emitting = effectSource;
	}
}