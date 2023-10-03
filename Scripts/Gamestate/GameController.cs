using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;
using Kompas.UI;

namespace Kompas.Gamestate
{
	public partial class GameController : Node
	{

		[Export]
		public PlayerController[] PlayerControllers { get; private set; }

		[Export]
		public BoardController BoardController { get; private set; }

		[Export]
		public GameUIController UIController { get; private set; }
	}
}