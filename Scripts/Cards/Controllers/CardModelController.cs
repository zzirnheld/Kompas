using Godot;
using Kompas.Cards.Views;
using Kompas.Shared.Exceptions;

namespace Kompas.Cards.Controllers
{
	public partial class CardModelController : Node3D
	{
		[Export]
		private Zoomable3DCardInfoDisplayer? _infoDisplayer;
		public Zoomable3DCardInfoDisplayer InfoDisplayer => _infoDisplayer
			?? throw new UnassignedReferenceException(nameof(_infoDisplayer), this);

		[Export]
		private CardMouseController? _mouseController;
		public CardMouseController MouseController => _mouseController
			?? throw new UnassignedReferenceException(nameof(_mouseController), this);

		[Export]
		private CardAugmentsController? _augmentsController;
		public CardAugmentsController AugmentsController => _augmentsController
			?? throw new UnassignedReferenceException(nameof(_augmentsController), this);

		[Export]
		private Node3D? _cameraPosition;
		public Node3D CameraPosition => _cameraPosition
			?? throw new UnassignedReferenceException(nameof(_cameraPosition), this);
	}
}