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
		private SpacesController? _spacesController;
		private SpacesController SpacesController => _spacesController
			?? throw new System.NullReferenceException("Forgot to init");

		public override void Place(ICardController cardController)
		{
			SpacesController.Place(cardController);
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
			bool normal = cardController == cardController.Card.Card.NormalCardController;
			foreach (var adjacentCard in cardController.Card.Game.Board.CardsAdjacentTo(cardController.Card.Position))
			{
				if (normal) ScaleCard(adjacentCard.NormalCardController);
				else ScaleCard(adjacentCard.IllusoryCardController);
			}
		}

		private static void ScaleCard(ICardController cardController)
		{
			cardController.Node.Scale = Vector3.One * SurroundedCardScale;
		}
	}
}