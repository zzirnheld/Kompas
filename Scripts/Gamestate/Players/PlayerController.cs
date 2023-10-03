using Godot;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Players
{
	public partial class PlayerController : Node
	{
		[Export]
		private HandController HandController { get; set; }

		[Export]
		private DiscardController DiscardController { get; set; }

		[Export]
		private DeckController DeckController { get; set; }

		[Export]

		private AnnihilationController AnnihilationController { get; set; }
	}
}