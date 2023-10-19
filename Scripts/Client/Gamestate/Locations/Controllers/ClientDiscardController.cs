using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDiscardController : DiscardController
	{
		[Export]
		private SplayOutController SplayOutController { get; set; }

		protected override void SpreadOut() => SplayOutController.SplayOut();

	}
}