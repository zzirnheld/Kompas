using Godot;
using Kompas.Client.Gamestate.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class SpacesController : Node3D
	{
		[Export]
		private SpaceController[] InitialSpaces { get; set; }

		[Export]
		private PackedScene Space { get; set; }

		private readonly SpaceController[,] spaces = new SpaceController[7, 7];

		public override void _Ready()
		{
			base._Ready();

			foreach (var space in InitialSpaces) Dupe(space);
		}

		private void Dupe(SpaceController space)
		{
			spaces[space.X, space.Y] = space;
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
			var newSpace = toDupe.Dupe(this, flipX, flipY, swapXY, (x, y) => spaces[x, y] == null, Space);
			if (newSpace != null) spaces[newSpace.X, newSpace.Y] = newSpace;
		}
	}
}