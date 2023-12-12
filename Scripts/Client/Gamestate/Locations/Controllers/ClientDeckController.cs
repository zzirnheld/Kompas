using System.Linq;
using Godot;
using Kompas.Client.UI;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDeckController : DeckController
	{
		private const string LookingAtAnimationName = "Looking At";
		private const string ResetAnimationName = "RESET";

		[Export]
		private SpiralController? _spiralController;
		private SpiralController SpiralController => _spiralController
			?? throw new UnassignedReferenceException();

		[Export]
		private ClientCameraController? _camera;
		private ClientCameraController Camera => _camera
			?? throw new UnassignedReferenceException();

		[Export]
		private AnimationPlayer? _animationPlayer;
		private AnimationPlayer AnimationPlayer => _animationPlayer
			?? throw new UnassignedReferenceException();

		public override void _Ready()
		{
			base._Ready();
			Camera.Arrived += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Arrived(); };
			Camera.Departed += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Departed(); };
		}

		private void Arrived()
		{
			AnimationPlayer.Play(LookingAtAnimationName);
		}

		private void Departed()
		{
			AnimationPlayer.Play(ResetAnimationName);
		}

		protected override void SpreadOut()
		{
			foreach (var card in DeckModel.Cards) card.CardController.Node.Visible = true;//false;
			SpiralController.SpiralOut(DeckModel.Cards.Select(c => c.CardController.Node).ToArray());
		}
	}
}