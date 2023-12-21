using System.Collections.Generic;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate.Controllers;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class SpacesController : Node3D
	{
		[Export]
		private SpaceController[]? _initialSpaces;
		private SpaceController[] InitialSpaces => _initialSpaces ?? throw new UnassignedReferenceException();

		[Export]
		private PackedScene? _space;
		private PackedScene Space => _space ?? throw new UnassignedReferenceException();

		[Export]
		private ClientGameController? _gameController;
		private ClientGameController GameController => _gameController ?? throw new UnassignedReferenceException();
		//if this becomes a shared controller, this will need to be moved to a child class

		private readonly SpaceController[,] spaces = new SpaceController[7, 7];
		public IEnumerable<ISpaceTargetingController> SpaceTargets
		{
			get
			{
				foreach (var space in spaces) yield return space.SpaceTargetingController;
			}
		}

		public SpaceController this[int x, int y] => spaces[x, y];
		public SpaceController this[Space space] => this[space.x, space.y];

		public override void _Ready()
		{
			base._Ready();

			foreach (var space in InitialSpaces) Dupe(space);
		}

		private void Dupe(SpaceController space)
		{
			InsertSpace(space);
			InsertSpace(space, true, false, false);
			InsertSpace(space, false, true, false);
			InsertSpace(space, true, true, false);
			InsertSpace(space, false, false, true);
			InsertSpace(space, false, true, true);
			InsertSpace(space, true, false, true);
			InsertSpace(space, true, true, true);
		}

		private void InsertSpace(SpaceController toDupe, bool flipX, bool flipY, bool swapXY)
		{
			InsertSpace(toDupe.Dupe(this, Space, flipX, flipY, swapXY, (x, y) => spaces[x, y] == null));
		}

		private void InsertSpace(SpaceController? newSpace)
		{
			if (newSpace == null) return;
			if (spaces[newSpace.X, newSpace.Y] != null) throw new System.InvalidOperationException($"{newSpace.X}, {newSpace.Y} already had a Space created for it!");

			spaces[newSpace.X, newSpace.Y] = newSpace;
			newSpace.LeftClick += (_, _) => Clicked(newSpace.X, newSpace.Y);
		}

		public void Clicked(int x, int y) => GameController.TargetingController.Select((x, y));
	}
}