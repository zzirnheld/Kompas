using Godot;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Players
{
	public partial class PlayerController : Node
	{
		[Export]
		private HandController? _handController;
		public HandController HandController => _handController ?? throw new UnassignedReferenceException();
		[Export]
		private HandController? _illusoryHandController;
		public HandController IllusoryHandController => _illusoryHandController ?? throw new UnassignedReferenceException();

		[Export]
		private DiscardController? _discardController;
		public DiscardController DiscardController => _discardController ?? throw new UnassignedReferenceException();
		[Export]
		private DiscardController? _illusoryDiscardController;
		public DiscardController IllusoryDiscardController => _illusoryDiscardController ?? throw new UnassignedReferenceException();

		[Export]
		private DeckController? _deckController;
		public DeckController DeckController => _deckController ?? throw new UnassignedReferenceException();
		[Export]
		private DeckController? _illusoryDeckController;
		public DeckController IllusoryDeckController => _illusoryDeckController ?? throw new UnassignedReferenceException();

		[Export]
		private AnnihilationController? _annihilationController;
		public AnnihilationController AnnihilationController => _annihilationController ?? throw new UnassignedReferenceException();

		public virtual IGameCardInfo Avatar { set { } }
		public virtual int Pips { set { } }
		public virtual int PipsNextTurn { set { } }
	}
}