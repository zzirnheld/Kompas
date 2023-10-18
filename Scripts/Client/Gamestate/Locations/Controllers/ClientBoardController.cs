using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientBoardController : BoardController
	{
		[Export]
		private SpacesController SpacesController { get; set; }

		public override void Place(ICardController cardController)
		{
			SpacesController[cardController.Card.Position].Place(cardController);
		}
	}
}