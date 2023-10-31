using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientBoardController : BoardController
	{
		private const float UnsurroundedCardScale = 1.55f;
		private const float SurroundedCardScale = 1.15f;

		[Export]
		private SpacesController SpacesController { get; set; }

		public override void Place(ICardController cardController)
		{
			SpacesController[cardController.Card.Position].Place(cardController);
			ScaleCard(cardController);
			ScaleAdjacentCards(cardController);
		}

		public override void Remove(ICardController cardController)
		{
			cardController.Node.Scale = Vector3.One;
			ScaleAdjacentCards(cardController);
		}

		private static void ScaleAdjacentCards(ICardController cardController)
		{
			foreach (var adjacentCard in cardController.Card.Game.Board.CardsAdjacentTo(cardController.Card.Position))
				ScaleCard(adjacentCard.CardController);
		}

		private static void ScaleCard(ICardController cardController)
		{
			cardController.Node.Scale = Vector3.One * (cardController.Card.IsAdjacentTo(c => c != null) ? SurroundedCardScale : UnsurroundedCardScale);
		}
	}
}