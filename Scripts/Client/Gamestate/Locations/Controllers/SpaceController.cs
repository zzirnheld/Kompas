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

		//Assumes 
		public SpaceController Dupe(Node3D parent, bool flipX, bool flipY, bool swapXY)
		{
			var ret = Duplicate() as SpaceController;
			parent.AddChild(ret);

			var (tempX, tempY) = (flipX ? BoardMax - X : X, 
								  flipY ? BoardMax - Y : Y);
			var (x, y) = swapXY ? (tempY, tempX) : (tempX, tempY);
			ret.X = x;
			ret.Y = y;

			var (tempXPos, tempZPos) = (flipX ? -Position.X : Position.X,
										flipY ? -Position.Z : Position.Z);

			var (xPos, zPos) = swapXY ? (tempZPos, tempXPos) : (tempXPos, tempZPos);

			ret.Position = new Vector3(xPos, Position.Y, zPos);
			return ret;
		}
	}
}