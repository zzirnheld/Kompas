using Godot;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Controllers
{
	public partial class SpaceTargetingController : Node, ISpaceTargetingController
	{
		[Export]
		private SpaceController? _spaceController;
		private SpaceController SpaceController => _spaceController ?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _canMove;
		private Node3D CanMove => _canMove ?? throw new UnassignedReferenceException();

		public Space Space => SpaceController.Space;

		public void ShowCanMoveHere(bool can)
		{
			CanMove.Visible = can;
		}
	}
}