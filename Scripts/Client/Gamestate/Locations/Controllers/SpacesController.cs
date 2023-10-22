using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate.Controllers;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class SpacesController : Node3D
	{
		[Export]
		private SpaceController[] InitialSpaces { get; set; }

		[Export]
		private PackedScene Space { get; set; }

		[Export]
		private ClientGameController GameController { get; set; } //if this becomes a shared controller, this will need to be moved to a child class

		private readonly SpaceController[,] spaces = new SpaceController[7, 7];

		public SpaceController this[int x, int y] => spaces[x, y];
		public SpaceController this[Space space] => this[space.x, space.y];

		public override void _Ready()
		{
			base._Ready();

			foreach (var space in InitialSpaces) Dupe(space);
		}

		private void Dupe(SpaceController space)
		{
			spaces[space.X, space.Y] = space; //TODO add handlers to the spaces being duplicated
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
			if (newSpace != null)
			{
				spaces[newSpace.X, newSpace.Y] = newSpace;
				newSpace.LeftClick += (_, _) => Clicked(newSpace.X, newSpace.Y);
			}
		}

		public void Clicked(int x, int y)
		{
			GameController.TargetingController.Select((x, y));
		}
	}
}