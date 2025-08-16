using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Client.UI;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers;

public partial class ClientDeckController : DeckController
{
	[Export]
	private GridArranger? _cardArranger;
	private GridArranger CardArranger => _cardArranger
		?? throw new UnassignedReferenceException(nameof(_cardArranger), this);

	[Export]
	private ClientCameraController? _cameraController;
	private ClientCameraController CameraController => _cameraController
		?? throw new UnassignedReferenceException(nameof(_cameraController), this);

	[Export]
	private ClientTargetingController? _targetingController;
	private ClientTargetingController TargetingController => _targetingController
		?? throw new UnassignedReferenceException(nameof(_targetingController), this);

	private ISet<int> lastArrangedCards = new HashSet<int>();

	public override void _Ready()
	{
		base._Ready();
		CameraController.StartedMovingTowards += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Arrived(); };
		CameraController.Departed += (_, at) => { if (DeckModel.IsLocation(at.Location, at.Friendly)) Departed(); };
		CardArranger.Close();
	}

	private void Arrived()
	{
		CardArranger.Open();
		Refresh();
	}

	private void Departed() => CardArranger.Close();

	protected override void SpreadOut()
	{
		if (!CardArranger.IsOpen)
		{
			CardArranger.TakeWithoutArranging(DeckModel.Cards.Select(c => c.CardController.Node).ToArray());
			return;
		}

		var cardsToArrange = DeckModel.Cards;
		if (TargetingController.Searching()) cardsToArrange = cardsToArrange.Where(TargetingController.IsBeingSearched).ToArray();

		var cardsToArrangeSet = new HashSet<int>(cardsToArrange.Select(card => card.ID));
		if (lastArrangedCards.SetEquals(cardsToArrangeSet)) return;

		lastArrangedCards = cardsToArrangeSet;
		var nodesToArrange = cardsToArrange
			.Select(c => c.CardController.Node)
			.ToArray();
		CardArranger.Arrange(nodesToArrange);
	}
}