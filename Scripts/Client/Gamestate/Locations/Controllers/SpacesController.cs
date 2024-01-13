using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate.Controllers;
using Kompas.Gamestate;
using Kompas.Shared.Enumerable;
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

			CanMove.Display(_ => false, false);
			CanPlay.Display(_ => false, false);

			CanMove.UpdateMaterial(CanMoveMaterial);
			CanPlay.UpdateMaterial(CanPlayMaterial);

			CanPlay.Display(space => space.DistanceTo((2, 1)) < 3, false);
		}

		public void Clicked(int x, int y) => GameController.TargetingController.Select((x, y));
	}
}