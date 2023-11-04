using System.Linq;
using Godot;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientDeckController : DeckController
	{
		[Export]
		private SplayOutController SplayOutController { get; set; }

		protected override void SpreadOut()
		{
			foreach (var card in DeckModel.Cards) card.CardController.Node.Visible = true;//false;
			SplayOutController.SplayOut(DeckModel.Cards.Select(c => c.CardController.Node).ToArray());
		}
	}
}