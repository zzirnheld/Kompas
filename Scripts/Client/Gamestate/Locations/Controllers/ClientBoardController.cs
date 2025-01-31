using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers;

public partial class ClientBoardController : BoardController
{
	private const float UnsurroundedCardScale = 1.55f;
	private const float SurroundedCardScale = 1.15f;

	[Export]
	private SpacesController? _spacesController;
	private SpacesController SpacesController => _spacesController
		?? throw new UnassignedReferenceException(nameof(_spacesController));

	[Export]
	private ClientTargetingController? _targetingController;
	private ClientTargetingController TargetingController => _targetingController
		?? throw new UnassignedReferenceException(nameof(_targetingController));


	public override void Play(ICardController cardController)
	{
		cardController.MoveToBoard(() =>
		{
			SpacesController.Play(cardController);
			ScaleCard(cardController);
			ScaleAdjacentCards(cardController);
		});
	}

	public override void Move(ICardController card, MovePath path)
	{
		SpacesController.Move(card, path);
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
		cardController.Node.Scale = Vector3.One * SurroundedCardScale;
	}
}