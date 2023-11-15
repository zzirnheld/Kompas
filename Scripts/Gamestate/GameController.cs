using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;
using Kompas.UI;

namespace Kompas.Gamestate
{
	public abstract partial class GameController : Node
	{
		[Export]
		public PlayerController[]? PlayerControllers { get; private set; }

		[Export]
		public BoardController? BoardController { get; private set; }

		public abstract IGame? Game { get; }
	}
}