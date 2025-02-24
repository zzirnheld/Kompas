using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate;

public abstract partial class GameController : Node, IGameController
{
	[Export]
	private PlayerController[]? _playerControllers;
	public IPlayerController[] PlayerControllers => _playerControllers
		?? throw new UnassignedReferenceException();

	[Export]
	private BoardController? _boardController;
	public IBoardController BoardController => _boardController
		?? throw new UnassignedReferenceException();

	public abstract IGame Game { get; }
}

public interface IGameController
{
	public IPlayerController[] PlayerControllers { get; }
	public IBoardController BoardController { get; }
	public IGame Game { get; }
}