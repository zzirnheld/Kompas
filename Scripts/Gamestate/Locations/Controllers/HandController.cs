using Godot;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Locations.Controllers
{
	public partial class HandController : Node
	{
		public HandModel HandModel { get; init; }

		public void Refresh() => SpreadOutCards();

		//TODO
		public void SpreadOutCards()
		{
			throw new System.NotImplementedException();
			//leftDummy.transform.localPosition = new Vector3(2.25f * (((float)hand.Count / -2f) + -1f + 0.5f), 0, 0);
			//rightDummy.transform.localPosition = new Vector3(2.25f * (((float)hand.Count / -2f) + (float)hand.Count + 0.5f), 0, 0);
			//iterate through children, set the z coord
				/*
			for (int i = 0; i < HandModel.HandSize; i++)
			{
				hand[i].CardController.transform.parent = transform;
				float offset = ((float)hand.Count / -2f) + (float)i + 0.5f;
				hand[i].CardController.transform.localPosition = new Vector3(2.25f * offset, 0, 0);
				hand[i].CardController.SetRotation();
				hand[i].CardController.gameObject.SetActive(true);
			}*/
		}
	}
}