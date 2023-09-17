using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using System.Linq;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderSearchController : Node
	{
		[Export]
		private DeckBuilderController DeckBuilderController { get; set; }
		[Export]
		private PackedScene SearchCardControllerPrefab { get; set; }
		[Export]
		private Control SearchGridParent { get; set; }

		private string lastSearch = string.Empty;

		public void Search(string basicText)
		{
			GD.Print($"Searching {basicText}");
			if (basicText.Length < 3)
			{
				Clear();
				return;
			}

			//TODO smarter: don't clear cards we still wanna show? if (basicText.Length <= lastSearch.Length || basicText[..lastSearch.Length] != lastSearch) Clear();

			Clear();

			bool IsValid(SerializableCard sCard)
			{
				return sCard.cardName.ToLower().Contains(basicText.ToLower())
					|| sCard.subtypeText.ToLower().Contains(basicText.ToLower())
					|| sCard.effText.ToLower().Contains(basicText.ToLower());
			}
			foreach (var sCard in CardRepository.SerializableCards.Where(IsValid)) ShowInSearch(sCard);
		}

		private void Clear()
		{
			foreach (var obj in SearchGridParent.GetChildren()) obj.QueueFree();
		}

		private void ShowInSearch(SerializableCard serializableCard)
		{
			GD.Print($"Showing {serializableCard}");
			var card = DeckBuilderCardRepository.CreateDeckBuilderCard(serializableCard);
			ShowInSearch(card);
		}

		//TODO - this is duplicated with DeckBuilderDeckController. consider deduplicating? (the logic around creating the view/controller)
		private void ShowInSearch(DeckBuilderCard card)
		{
			var ctrl = CreateCardController();
			SearchGridParent.AddChild(ctrl);
			ctrl.Init(card, DeckBuilderController.CardView, DeckBuilderController.DeckController);
		}

		private DeckBuilderCardController CreateCardController()
		{
			if (SearchCardControllerPrefab.Instantiate() is not DeckBuilderCardController controller)
					throw new System.ArgumentNullException(nameof(SearchCardControllerPrefab), "Was not the right type");
			return controller;
		}
	}
}