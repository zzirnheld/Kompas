using Godot;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class LinkedSpaceController : Node3D
	{
		[Export]
		private Node3D? _tile;
		public Node3D Tile => _tile ?? throw new UnassignedReferenceException();

		[Export]
		public Node3D? Plus1X { get; private set; }
		[Export]
		public Node3D? Plus1Y { get; private set; }
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
	}
}