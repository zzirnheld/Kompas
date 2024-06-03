using Godot;
using Kompas.Godot;
using Kompas.Shared.Enumerable;
using System;
using System.Collections.Generic;

namespace Kompas.Cards.Controllers
{
	public partial class CardAugmentsController : Node
	{
		private const float VerticalStackIncrement = 0.05f;

		public void Stack(IEnumerable<ICardController> cards)
		{
			foreach (var (index, card) in cards.Enumerate())
			{
				var node = card.Node
					?? throw new System.NullReferenceException("ClientCardController must have non-null nodes!");
				this.TransferChild(node);
				node.Visible = true;
				node.Scale = Vector3.One * 0.2f;
				var rotation = card.Card.ControllingPlayer.Index * Mathf.Pi;
				node.Rotation = new Vector3(0, rotation, 0);
				node.Position = (Vector3.Up * VerticalStackIncrement)
							  + (Vector3.Right * index * 0.05f); //TODO better spread
			}
		}

		public void Spread(IEnumerable<ICardController> cards)
		{

		}
	}
}