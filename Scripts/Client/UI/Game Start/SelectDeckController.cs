using System.Collections.Generic;
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
		private SelectDeckCardController AvatarController { get; set; }

		[Export]
		private PackedScene MainDeckCardPrefab { get; set; }

		[Export]
		private ClientGameController GameController { get; set; }

		private readonly IList<string> deckNames = new List<string>();

		public override void _Ready()
		{
			foreach (var deckName in DeckAccess.GetDeckNames()) AddDeckName(deckName);

			//TODO handle having no decks and trying to enter client - error and boot back to main menu

			Load(0);
		}

		private void AddDeckName(string deckName)
		{
			deckNames.Add(deckName);
			DeckSelect.AddItem(deckName);
		}

		private void Load(int index)
		{
			var decklist = DeckAccess.Load(deckNames[index]);
			ShowDeck(decklist);
		}

		private void ShowDeck(Decklist decklist)
		{
			//ClearDeck();
			foreach (var cardName in decklist.deck)
			{
				var card = GameController.CardRepository.InstantiateDeckSelectCard(cardName);
				var ctrl = CreateCardController();
				ctrl.Init(card);

				GD.Print($"Loaded {cardName}");

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