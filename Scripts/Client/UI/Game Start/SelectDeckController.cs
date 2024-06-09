using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate;
using Kompas.Godot;
using Kompas.Shared;
using Kompas.Shared.Exceptions;
using Kompas.UI;

namespace Kompas.Client.UI.GameStart
{
	public partial class SelectDeckController : Control
	{
		[Export]
		private OptionButton? _deckSelect;
		private OptionButton DeckSelect => _deckSelect ?? throw new UnassignedReferenceException();

		[Export]
		private SquareGridContainer? _deckContainer;
		private SquareGridContainer DeckContainer => _deckContainer ?? throw new UnassignedReferenceException();

		[Export]
		private SelectDeckCardController? _avatarController;
		private SelectDeckCardController AvatarController => _avatarController ?? throw new UnassignedReferenceException();

		[Export]
		private PackedScene? _mainDeckCardPrefab;
		private PackedScene MainDeckCardPrefab => _mainDeckCardPrefab ?? throw new UnassignedReferenceException();

		[Export]
		private GameStartController? _gameStartController;
		private GameStartController GameStartController => _gameStartController ?? throw new UnassignedReferenceException();

		private readonly IList<string> deckNames = new List<string>();

		private DeckAccess? deckLoader;
		private Decklist? selectedDeck;

		//Event handler - when controller becomes ready.
		public override void _Ready()
		{
			DeckSelect.Clear();
		}

		public async Task Init()
		{
			await Task.Delay(1000);
			deckLoader = await Task.Run(DeckAccess.Create);
			foreach (var deckName in deckLoader.DeckNames) AddDeckName(deckName);

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
			_ = deckLoader ?? throw new NotInitializedException();

			selectedDeck = deckLoader.Load(deckNames[index]);
			if (selectedDeck == null)
			{
				Logger.Err($"No deck found for {deckNames[index]}");
				return;
			}
			ShowDeck(selectedDeck);
		}

		private void ShowDeck(Decklist decklist)
		{
			_ = deckLoader ?? throw new NotInitializedException();

			ClearDeck();
			foreach (var cardName in decklist.deck)
			{
				var card = GameStartController.GameController.CardRepository.InstantiateDeckSelectCard(cardName);
				if (card == null)
				{
					Logger.Err($"Couldn't init card {cardName}");
					continue;
				}
				var ctrl = CreateCardController();
				ctrl.Init(card);

				Logger.Log($"Loaded {cardName}");

				DeckContainer.AddChild(ctrl);
			}

			string avatarName = decklist.avatarName ?? throw new NullReferenceException();
			var avatar = GameStartController.GameController.CardRepository.InstantiateDeckSelectCard(avatarName);
			if (avatar == null)
			{
				Logger.Err($"Couldn't init avatar {decklist.avatarName}");
				return;
			}
			AvatarController.Init(avatar);
		}

		private void ClearDeck()
		{
			DeckContainer.RemoveAndQueueFreeChildren();
		}

		private SelectDeckCardController CreateCardController()
		{
			if (MainDeckCardPrefab.Instantiate() is not SelectDeckCardController controller)
					throw new System.ArgumentNullException(nameof(MainDeckCardPrefab), "Was not the right type");
			return controller;
		}

		public void SelectDeck()
		{
			if (selectedDeck == null)
			{
				Logger.Err($"No deck selected!");
				return;
			}

			GameStartController.GameController.Notifier.RequestDecklistImport(selectedDeck);
			GameStartController.DeckSubmitted();
		}
	}
}