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
		[Export]
		private GridArranger? _cardArranger;
		private GridArranger CardArranger => _cardArranger
			?? throw new UnassignedReferenceException();

		[Export]
		private ClientCameraController? _cameraController;
		private ClientCameraController CameraController => _cameraController
			?? throw new UnassignedReferenceException();

		public override void _Ready()
		{
			base._Ready();
			CameraController.Arrived += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Arrived(); };
			CameraController.Departed += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Departed(); };
			CardArranger.Close();
		}

		private void Arrived() => CardArranger.Open();

		private void Departed() => CardArranger.Close();

		protected override void SpreadOut()
		{
			CardArranger.Arrange(DeckModel.Cards.Select(c => c.CardController.Node).ToArray());
		}
	}
}