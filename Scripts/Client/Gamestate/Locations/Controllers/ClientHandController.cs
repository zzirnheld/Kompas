using System.Linq;
using Godot;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientHandController : HandController
	{
		private const float CardOffset = 2.25f;

		protected override void SpreadAllCards()
		{
			GD.Print($"Spreading {string.Join(", ", HandModel.Cards.Select(c => c.CardName))}");
			for (int i = 0; i < HandModel.HandSize; i++)
			{
				var node = HandModel[i].CardController.Node;
				AddChild(node);

				//Offset = (card's index) - (1/2 * # cards in hand)
				float offsetMultiplier = i - (HandModel.HandSize / 2f);
				node.Position = Vector3.Left * (CardOffset * offsetMultiplier);
				
				node.Rotation = Vector3.Zero;
				node.Visible = true;
				GD.Print($"Placing {HandModel[i].CardName} at {node.Position}");
			}
		}
	}
}