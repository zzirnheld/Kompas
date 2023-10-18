using Godot;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Controllers
{
	public partial class SpaceController : Node3D
	{
		private const int BoardMax = 6;

		[Export]
		public int X { get; private set; }
		[Export]
		public int Y { get; private set; }

		[Export]
		private Node3D CanPlayTo { get; set; }

		public override void _Ready()
		{
			base._Ready();
			//CanPlayTo.Visible = false;
		}

		//Assumes 
		public SpaceController Dupe(Node3D parent, bool flipX, bool flipY, bool swapXY)
		{
			var ret = Duplicate() as SpaceController;
			parent.AddChild(ret);

			var (x, y) = (flipX ? BoardMax - X : X, 
						  flipY ? BoardMax - Y : Y);
			(ret.X, ret.Y) = swapXY ? (y, x) : (x, y);

			var (tempXPos, tempZPos) = (flipX ? -Position.X : Position.X,
										flipY ? -Position.Z : Position.Z);
			var (xPos, zPos) = swapXY ? (tempZPos, tempXPos) : (tempXPos, tempZPos);
			GD.Print($"{X}, {Y} with {flipX}\t{flipY}\t{swapXY} a space at {ret.X},{ret.Y} at {xPos}, {zPos}");
			ret.Position = new Vector3(xPos, Position.Y, zPos);
			return ret;
		}
	}
}