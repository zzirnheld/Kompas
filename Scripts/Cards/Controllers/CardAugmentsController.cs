using Godot;
using Kompas.Godot;
using Kompas.Shared.Enumerable;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Cards.Controllers;

//FUTURE: if I don't like that the augments are moved around with the currently selected card,
//will need to move the augment controller out of the card model controller
//very doable, fortunately
public partial class CardAugmentsController : Node
{
	private const float VerticalStackBaseHeight = 0.05f;
	private const float VerticalStackIncrement = 0.1f;
	private const float VerticalSpreadHeight = 0.25f;
	
	private const float HorizontalStackIncrement = 0.1f;
	private const float HorizontalSpreadRadius = 1f;

	private const float AugmentScaleInStack = 0.75f;
	private const float AugmentScaleInSpread = AugmentScaleInStack;

	private const float SpreadAngleStartOffset = Mathf.Pi / 4f;
	private static readonly Vector3 TurnAroundRotation = new(0, Mathf.Pi, 0);

	/// <summary>
	/// IMPL NOTE: Does not remove/free children.
	/// Children should be transferred/freed by the Remove method and its consequences.
	/// </summary>
	public void Stack(IEnumerable<ICardController> cards)
	{
		foreach (var (index, card) in cards.Enumerate())
		{
			var node = card.Node
				?? throw new System.NullReferenceException("To stack augments, card controllers must have non-null nodes!");
			this.TransferChild(node);

			node.Visible = true;
			node.Scale = Vector3.One * AugmentScaleInStack;

			node.Rotation = card.Card.ControllingPlayer.Index * TurnAroundRotation;

			node.Position = (Vector3.Up * ((index * VerticalStackIncrement) + VerticalStackBaseHeight))
						  + (Vector3.Right * index * HorizontalStackIncrement);
		}
	}

	public void Spread(IEnumerable<ICardController> cards)
	{
		var cardsArr = cards.ToArray();
		int count = cardsArr.Length;

		foreach (var (index, card) in cardsArr.Enumerate())
		{
			var node = card.Node
				?? throw new System.NullReferenceException("To stack augments, card controllers must have non-null nodes!");
			this.TransferChild(node);

			node.Visible = true;
			node.Scale = Vector3.One * AugmentScaleInSpread;

			node.Rotation = card.Card.ControllingPlayer.Index * TurnAroundRotation;

			float proportion = (float)index / count;
			var spreadAngle = (2 * Mathf.Pi * proportion) + SpreadAngleStartOffset;
			node.Position = (Vector3.Up * VerticalSpreadHeight)
						  + (Vector3.Right * HorizontalSpreadRadius * Mathf.Cos(spreadAngle))
						  + (Vector3.Back * HorizontalSpreadRadius * Mathf.Sin(spreadAngle));
		}
	}
}