using System;
using System.Collections.Generic;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate;
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

		public override void _Ready()
		{
			DeckSelect.Clear();
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
			if (decklist == null)
			{
				GD.PushError($"No deck found for {deckNames[index]}");
				return;
			}
			ShowDeck(decklist);
		}

		private void ShowDeck(Decklist decklist)
		{
			ClearDeck();
			foreach (var cardName in decklist.deck)
			{
				var card = GameStartController.GameController.CardRepository.InstantiateDeckSelectCard(cardName);
				if (card == null)
				{
					GD.PushError($"Couldn't init card {cardName}");
					continue;
				}
				var ctrl = CreateCardController();
				ctrl.Init(card);

				GD.Print($"Loaded {cardName}");

				DeckContainer.AddChild(ctrl);
			}

			string avatarName = decklist.avatarName ?? throw new NullReferenceException();
			var avatar = GameStartController.GameController.CardRepository.InstantiateDeckSelectCard(avatarName);
			if (avatar == null)
			{
				GD.PushError($"Couldn't init avatar {decklist.avatarName}");
				return;
			}
			AvatarController.Init(avatar);
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

		public void SelectDeck()
		{
			var decklist = DeckAccess.Load(deckNames[DeckSelect.Selected]);
			if (decklist == null)
			{
				GD.PushError($"No deck found for {deckNames[DeckSelect.Selected]}");
				return;
			}
			GameStartController.GameController.Notifier.RequestDecklistImport(decklist);
			GameStartController.DeckSubmitted();
		}
	}
}