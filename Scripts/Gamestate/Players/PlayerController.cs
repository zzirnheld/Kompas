using Godot;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Players;

public partial class PlayerController : Node, IPlayerController
{
	[Export]
	private HandController? _handController;
	public IHandController HandController => _handController ?? throw new UnassignedReferenceException();

	[Export]
	private DiscardController? _discardController;
	public IDiscardController DiscardController => _discardController ?? throw new UnassignedReferenceException();

	[Export]
	private DeckController? _deckController;
	public IDeckController DeckController => _deckController ?? throw new UnassignedReferenceException();

	[Export]
	private AnnihilationController? _annihilationController;
	public IAnnihilationController AnnihilationController => _annihilationController ?? throw new UnassignedReferenceException();

	public virtual IGameCardInfo Avatar { set { } }
	public virtual int Pips { set { } }
	public virtual int PipsNextTurn { set { } }
}

public interface IPlayerController
{
	public IHandController HandController { get; }
	public IDiscardController DiscardController { get; }
	public IDeckController DeckController { get; }
	public IAnnihilationController AnnihilationController { get; }

	public IGameCardInfo Avatar { set; }
	public int Pips { set; }
	public int PipsNextTurn { set; }
}