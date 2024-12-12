using System.Collections.Generic;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers;

public partial class PlaceInSpaceController : Node
{
	[Export]
	private Node? _positionsParent;
	private Node PositionsParent => _positionsParent ?? throw new UnassignedReferenceException();

	private IDictionary<Space, PlaceableSpaceController> SpaceToPosition { get; }
		= new Dictionary<Space, PlaceableSpaceController>();

	public override void _Ready()
	{
		foreach (var node in PositionsParent.GetChildren())
		{
			//TODO: get node child as thing with space
			//add to dict
			if (node is not PlaceableSpaceController space)
				throw new System.InvalidOperationException($"{node} was not a placeable space!");

			SpaceToPosition[space.Space] = space;
		}
	}

	public void Place(ICardController card)
	{
		var pos = card.Card.Position
			?? throw new System.InvalidOperationException($"Can't place {card} because its position is null!");
		Logger.Log($"Placing {card} in {pos}");
		SpaceToPosition[pos].Place(card);
	}

	public void Move(ICardController card, MovePath path) {
		var tween = CreateTween();
		foreach (var space in path.Spaces) {
			var pos = SpaceToPosition[space];
			tween.TweenCallback(Callable.From(() => pos.Take(card)));

			tween.TweenProperty(card.Node, "position", Vector3.Zero, 0.2d);
			tween.Parallel().TweenProperty(card.Node, "scale", Vector3.One, 0.2d);
		}
	}
}