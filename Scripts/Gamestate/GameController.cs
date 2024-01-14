using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;
using Kompas.Shared.Exceptions;
using Kompas.UI;

namespace Kompas.Gamestate
{
	public abstract partial class GameController : Node
	{
		[Export]
		private PlayerController[]? _playerControllers;
		public PlayerController[] PlayerControllers => _playerControllers
			?? throw new UnassignedReferenceException();

		[Export]
		private BoardController? _boardController;
		public BoardController BoardController => _boardController
			?? throw new UnassignedReferenceException();

		public abstract IGame Game { get; }
	}
}