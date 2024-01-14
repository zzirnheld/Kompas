using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class SpacesController : Node3D
	{
		[Export]
		private PackedScene? _linkedSpaces;
		private PackedScene LinkedSpaces => _linkedSpaces ?? throw new UnassignedReferenceException();

		[Export]
		private ClientGameController? _gameController;
		private ClientGameController GameController => _gameController ?? throw new UnassignedReferenceException();
		//if this becomes a shared controller, this will need to be moved to a child class

		[Export]
		private LinkedSpacesController? _canMove;
		private LinkedSpacesController CanMove => _canMove ?? throw new UnassignedReferenceException();
		[Export]
		private Material? _canMoveMaterial;
		private Material CanMoveMaterial => _canMoveMaterial ?? throw new UnassignedReferenceException();

		[Export]
		private LinkedSpacesController? _canPlay;
		private LinkedSpacesController CanPlay => _canPlay ?? throw new UnassignedReferenceException();
		[Export]
		private Material? _canPlayMaterial;
		private Material CanPlayMaterial => _canPlayMaterial ?? throw new UnassignedReferenceException();

		public override void _Ready()
		{
			base._Ready();

			CanMove.UpdateMaterial(CanMoveMaterial);
			CanPlay.UpdateMaterial(CanPlayMaterial);

			DisplayNone();
		}

		public void DisplayNone()
		{
			CanMove.Display(_ => false, false);
			CanPlay.Display(_ => false, false);
		}

		public void DisplayCanMove(LinkedSpacesController.ShouldShowSpace predicate)
		{
			CanMove.Display(predicate, false);
			CanPlay.Display(_ => false, false);
		}

		public void DisplayCanPlay(LinkedSpacesController.ShouldShowSpace predicate)
		{
			CanMove.Display(_ => false, false);
			CanPlay.Display(predicate, false);
		}

		public void Clicked(int x, int y) => GameController.TargetingController.Select((x, y));
	}
}