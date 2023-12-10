using System.Linq;
using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDeckController : DeckController
	{
		[Export]
		private SpiralController? _spiralController;
		private SpiralController SpiralController => _spiralController
			?? throw new UnassignedReferenceException();

		protected override void SpreadOut()
		{
			foreach (var card in DeckModel.Cards) card.CardController.Node.Visible = true;//false;
			SpiralController.SpiralOut(DeckModel.Cards.Select(c => c.CardController.Node).ToArray());
		}
	}
}