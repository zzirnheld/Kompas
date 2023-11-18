using System.Linq;
using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDiscardController : DiscardController
	{
		[Export]
		private SplayOutController? _splayOutController;
		private SplayOutController SplayOutController => _splayOutController
			?? throw new UnassignedReferenceException();

		protected override void SpreadOut()
			=> SplayOutController.SplayOut(DiscardModel.Cards.Select(c => c.CardController.Node).ToArray());

	}
}