using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Shared;
using Newtonsoft.Json;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderDeckController : Control
	{
		public const string DeckFolderPath = "user://Decks";
		private const string CurrentDeckGroupName = "CurrentDeck";

		//TODO factor out this tab ui behavior to make this class just responsible for CRUD-y stuff
		public enum Tab { Normal, NewDeck, SaveAs }

		[Export]
		private SquareGridContainer DeckNodesParent { get; set; }

		[Export]
		private PackedScene DeckCardControllerPrefab { get; set; }
		[Export]
		private DeckBuilderAvatarController AvatarController { get; set; }

		[Export]
		private DeckBuilderController DeckBuilderController { get; set; }

		[Export]
		private Control[] Tabs { get; set; }

		[Export]
		private OptionButton DeckNameSelect { get; set; }

		private readonly List<string> deckNames = new();

		private Decklist currentDeck;
		//To maintain ordering of decks where copies of the same card aren't next to each other
		private readonly List<DeckBuilderDeckCardController> currentDeckCtrls = new();

		public DeckBuilderDeckCardController Dragging { get; set; }


		private bool placeholdersWereActive;

		public override void _Ready()
		{
			EnsureDeckDirectory();
			using var folder = DirAccess.Open(DeckFolderPath);
			foreach (string deckFileName in folder.GetFiles())
			{
				if (deckFileName[^5..] != ".json") return; // GD.Print($"{deckFileName[^5..]}");
				string deckName = deckFileName[..^5];
				AddDeckName(deckName);
			}

			AvatarController.Init(null, DeckBuilderController.CardView, this);
			if (deckNames.Count == 0) ShowController(Tab.NewDeck);
			else LoadDeck(0);
		}

		public void ShowController(Tab toShow)
		{
			foreach (Tab tab in Enum.GetValues(typeof(Tab)))
			{
				Tabs[(int)tab].Visible = tab == toShow;
			}
		}

		public void NewDeck(string name)
		{
			SaveDeck();
			ClearDeck();

			currentDeck = new() { deckName = name };
			AddDeckNameAndSelect(name);
			SaveDeck();
		}

		public void SaveAs(string name)
		{
			currentDeck = currentDeck.Copy(name);
			AddDeckNameAndSelect(name);
			SaveDeck();
		}

		private void AddDeckNameAndSelect(string name)
		{
			AddDeckName(name);
			DeckNameSelect.Select(DeckNameSelect.ItemCount - 1);
		}

		private void ClearDeck()
		{
			foreach (var card in currentDeckCtrls.ToArray()) card.Delete();

			AvatarController.Clear();
			
			if (currentDeckCtrls.Count > 0)
			{
				GD.PrintErr("Didn't delete all cards succesfully!?");
				currentDeckCtrls.Clear();
			}
		}

		private void SaveDeck()
		{
			if (currentDeck == null) return;
			
			EnsureDeckDirectory();

			using var deck = FileAccess.Open($"{DeckFolderPath}/{currentDeck.deckName}.json", FileAccess.ModeFlags.Write);
			if (deck == null) GD.Print(FileAccess.GetOpenError());
			string json = JsonConvert.SerializeObject(currentDeck);
			deck.StoreString(json);
		}

		public void DeleteSelectedDeck()
		{
			using var deckFolder = DirAccess.Open(DeckFolderPath);
			if (deckFolder == null) GD.Print(DirAccess.GetOpenError());

			deckFolder.Remove($"{currentDeck.deckName}.json");

			int deckIndex = deckNames.IndexOf(currentDeck.deckName);
			DeckNameSelect.RemoveItem(deckIndex);
			deckNames.RemoveAt(deckIndex);

			//TODO handle deleting last deck. maybe force user onto the "create first deck" code path?
			//TODO create first deck code path that's new deck but you can't cancel out (forcing you to have at least 1 deck)
			int indexToSelect = deckIndex == 0 ? 0 : deckIndex - 1;
			LoadDeck(indexToSelect);
			DeckNameSelect.Select(indexToSelect);
		}

		public void LoadDeck(int index)
		{
			if (deckNames.Count <= index) return;
			LoadDeck(deckNames[index]);
		}

		private void LoadDeck(string deckName)
		{
			var path = $"{DeckFolderPath}/{deckName}.json";
			if (!FileAccess.FileExists(path)) return;

			ClearDeck();
			using var deck = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = deck.GetAsText();
			GD.Print($"Loading {json}");
			Decklist decklist = JsonConvert.DeserializeObject<Decklist>(json);

			if (decklist.avatarName != null)
			{
				var avatar = DeckBuilderCardRepository.CreateDeckBuilderCard(decklist.avatarName);
				AvatarController.UpdateAvatar(avatar);
			}

			if (decklist.deck != null)
				foreach (string cardName in decklist.deck) AddToDeck(cardName);

			currentDeck = decklist;
		}

		private void AddToDeck(string cardName)
		{
			var card = DeckBuilderCardRepository.CreateDeckBuilderCard(cardName);
			AddToDeck(card);
		}

		public void AddToDeck(DeckBuilderCard card)
		{
			var ctrl = CreateCardController();
			DeckNodesParent.AddChild(ctrl);
			//DeckNodesParent.MoveChild(ctrl, -1 - DeckSpacingPlaceholders.Length);
			ctrl.Init(card, DeckBuilderController.CardView, this);
			currentDeck?.deck.Add(card.CardName); //It's ok that we add to the decklist before replacing it in LoadDeck because it just gets garbage collected
			currentDeckCtrls.Add(ctrl);
		}

		public void RemoveFromDeck(DeckBuilderDeckCardController card)
		{
			int index = currentDeckCtrls.IndexOf(card);
			if (index < 0) return;

			DeckNodesParent.RemoveChild(card);
			currentDeck?.deck.RemoveAt(index);
			currentDeckCtrls.RemoveAt(index);
		}

		public void BecomeAvatar(DeckBuilderCardController card)
		{
			AvatarController.UpdateAvatar(card.Card);
			currentDeck.avatarName = card.Card.CardName;
		}

		public void DragSwap(DeckBuilderDeckCardController card)
		{
			if (Dragging == null) return;

			int argIndex = currentDeckCtrls.IndexOf(card);
			if (argIndex < 0) { GD.Print($"{card} not in deck"); return; }

			int draggingIndex = currentDeckCtrls.IndexOf(Dragging);

			if (!currentDeckCtrls.Remove(Dragging)) return;
			currentDeckCtrls.Insert(argIndex, Dragging);

			currentDeck.deck.RemoveAt(draggingIndex);
			currentDeck.deck.Insert(argIndex, Dragging.Card.CardName);

			DeckNodesParent.MoveChild(Dragging, argIndex);
		}

		private DeckBuilderDeckCardController CreateCardController()
		{
			if (DeckCardControllerPrefab.Instantiate() is not DeckBuilderDeckCardController controller)
					throw new System.ArgumentNullException(nameof(DeckCardControllerPrefab), "Was not the right type");
			return controller;
		}

		private static void EnsureDeckDirectory()
		{
			if (!DirAccess.DirExistsAbsolute(DeckFolderPath))
			{
				using var folder = DirAccess.Open("user://");
				folder.MakeDir(DeckFolderPath);
			}
		}

		private void AddDeckName(string deckName)
		{
			deckNames.Add(deckName);
			DeckNameSelect.AddItem(deckName);
		}
	}
}