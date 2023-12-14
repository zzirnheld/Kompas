using System.Linq;
using Godot;
using Kompas.Client.UI;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDiscardController : DiscardController
	{
		[Export]
		private NodeArranger? _cardArranger;
		private NodeArranger CardArranger => _cardArranger
			?? throw new UnassignedReferenceException();

		[Export]
		private ClientCameraController? _cameraController;
		private ClientCameraController CameraController => _cameraController
			?? throw new UnassignedReferenceException();

		public override void _Ready()
		{
			base._Ready();
			CameraController.Arrived += (_, at) => { if (DiscardModel.IsLocation(at.Location, at.Friendly)) Arrived(); };
			CameraController.Departed += (_, at) => { if (DiscardModel.IsLocation(at.Location, at.Friendly)) Departed(); };
		}

		private void Arrived() => CardArranger.Open();

		private void Departed() => CardArranger.Close();

		protected override void SpreadOut()
			=> CardArranger.Arrange(DiscardModel.Cards.Select(c => c.CardController.Node).ToArray());

	}
}