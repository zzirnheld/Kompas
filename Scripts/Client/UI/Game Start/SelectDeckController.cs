using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Client.Gamestate;
using Kompas.Shared;
using Kompas.UI;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.UI.GameStart
{
	public partial class SelectDeckController : Control
	{
		[Export]
		private OptionButton DeckSelect { get; set; }

		[Export]
		private SquareGridContainer DeckContainer { get; set; }

		[Export]
		private ControlInfoDisplayer AvatarInfoDisplayer { get; set; }

		[Export]
		private PackedScene MainDeckCardPrefab { get; set; }

		[Export]
		private ClientGameController GameController { get; set; }

		private void ShowDeck(Decklist decklist)
		{
			ClearDeck();
			foreach (var cardName in decklist.deck)
			{
				var card = GameController.CardRepository.InstantiateDeckSelectCard(CardRepository.GetJsonFromName(cardName));
				var ctrl = CreateCardController();
				ctrl.Init(card);

				DeckContainer.AddChild(ctrl);
			}
		}

		private void ClearDeck()
		{
			foreach (var child in DeckContainer.GetChildren()) child.QueueFree();
		}


		private SelectDeckCardController CreateCardController()
		{
			if (MainDeckCardPrefab.Instantiate() is not SelectDeckCardController controller)
					throw new System.ArgumentNullException(nameof(MainDeckCardPrefab), "Was not the right type");
			return controller;
		}
	}
}