using Godot;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class LinkedSpaceController : Node3D
	{
		[Export]
		private Node3D? _tile;
		private Node3D Tile => _tile ?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? Plus1X { get; set; }
		[Export]
		private Node3D? Plus1Y { get; set; }
		[Export]
		private int x;
		[Export]
		private int y;

		public Space Coords => (x, y);
		
		public override void _Ready()
		{
			if (Plus1X == null && x < Space.BoardLen - 1) throw new UnassignedReferenceException();
			if (Plus1Y == null && y < Space.BoardLen - 1) throw new UnassignedReferenceException();
		}

		public void Display(bool plusX, bool plusY)
		{
			Visible = true;
			if (plusX)
			{
				if (Plus1X == null) throw new UnassignedReferenceException();
				Plus1X.Visible = true;
			}
			if (plusY)
			{
				if (Plus1Y == null) throw new UnassignedReferenceException();
				Plus1Y.Visible = true;
			}
		}

		public void DisplayNone()
		{
			Visible = false;
		}
	}
}