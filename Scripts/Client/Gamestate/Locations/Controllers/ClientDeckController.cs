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
		private const string OpenAnimationName = "Open";
		private const string WhileOpenAnimationName = "Spin";
		private const string CloseAnimationName = "Close";

		[Export]
		private GridController? _gridController;
		private GridController GridController => _gridController
			?? throw new UnassignedReferenceException();

		[Export]
		private ClientCameraController? _cameraController;
		private ClientCameraController CameraController => _cameraController
			?? throw new UnassignedReferenceException();

		[Export]
		private AnimationPlayer? _animationPlayer;
		private AnimationPlayer AnimationPlayer => _animationPlayer
			?? throw new UnassignedReferenceException();

		public override void _Ready()
		{
			base._Ready();
			CameraController.Arrived += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Arrived(); };
			CameraController.Departed += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Departed(); };
		}

		private void Arrived()
		{
			AnimationPlayer.Play(OpenAnimationName);
			//AnimationPlayer.Queue(WhileOpenAnimationName);
		}

		private void Departed()
		{
			AnimationPlayer.Play(CloseAnimationName);
		}

		protected override void SpreadOut()
		{
			foreach (var card in DeckModel.Cards) card.CardController.Node.Visible = true;//false;
			GridController.Arrange(DeckModel.Cards.Select(c => c.CardController.Node).ToArray());
		}
	}
}