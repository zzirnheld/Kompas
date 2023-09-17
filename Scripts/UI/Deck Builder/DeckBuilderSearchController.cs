using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;
using System.Linq;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderSearchController : Node
	{
		[Export]
		private DeckBuilderController DeckBuilderController { get; set; }
		[Export]
		private PackedScene SearchInfoDisplayerPrefab { get; set; }
		[Export]
		private Control SearchGridParent { get; set; }

		private string lastSearch = string.Empty;

		public void Search(string basicText)
		{
			if (basicText.Length < 3)
			{
				Clear();
				return;
			}

			//TODO smarter: don't clear cards we still wanna show? if (basicText.Length <= lastSearch.Length || basicText[..lastSearch.Length] != lastSearch) Clear();

			Clear();

			bool IsValid(SerializableCard sCard)
			{
				return sCard.cardName.Contains(basicText)
					|| sCard.subtypeText.Contains(basicText)
					|| sCard.effText.Contains(basicText);
			}
			foreach (var sCard in CardRepository.SerializableCards.Where(IsValid)) ShowInSearch(sCard);
		}

		private void Clear()
		{
			foreach (var obj in SearchGridParent.GetChildren()) obj.QueueFree();
		}

		private void ShowInSearch(SerializableCard serializableCard)
		{
			var card = DeckBuilderCardRepository.CreateDeckBuilderCard(serializableCard);
			ShowInSearch(card);
		}

		//TODO - this is duplicated with DeckBuilderDeckController. consider deduplicating? (the logic around creating the view/controller)
		private void ShowInSearch(DeckBuilderCard card)
		{
			var infoDisplayer = CreateDeckInfoDisplayer();
			var view = new DeckBuilderCardView(infoDisplayer);
			var ctrl = new DeckBuilderCardController(view, card);
			SearchGridParent.AddChild(infoDisplayer);
			ctrl.Display();
		}

		private DeckBuilderSearchInfoDisplayer CreateDeckInfoDisplayer()
		{
			if (SearchInfoDisplayerPrefab.Instantiate() is not DeckBuilderSearchInfoDisplayer card)
				throw new System.ArgumentNullException(nameof(SearchInfoDisplayerPrefab), "Was not the right type");
			return card;
		}
	}
}