using Godot;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Players
{
	public partial class PlayerController : Node
	{
		[Export]
		public HandController HandController { get; private set; }

		[Export]
		public DiscardController DiscardController { get; private set; }

		[Export]
		public DeckController DeckController { get; private set; }

		[Export]

		public AnnihilationController AnnihilationController { get; private set; }

		public virtual IGameCard Avatar { set { } }
		public virtual int Pips { set { } }
		public virtual int PipsNextTurn { set { } }
	}
}