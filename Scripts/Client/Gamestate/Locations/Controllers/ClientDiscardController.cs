using System.Linq;
using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDiscardController : DiscardController
	{
		[Export]
		private SpiralController? _spiralController;
		private SpiralController SpiralController => _spiralController
			?? throw new UnassignedReferenceException();

		protected override void SpreadOut()
			=> SpiralController.SpiralOut(DiscardModel.Cards.Select(c => c.CardController.Node).ToArray());

	}
}