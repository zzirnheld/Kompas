using System;
using System.Collections.Generic;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Shared;
using Kompas.UI.CardInfoDisplayers;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;
using Newtonsoft.Json;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderDeckController : Control
	{
		public const string DeckFolderPath = "user://Decks";

		public enum Tab { Normal, NewDeck }

		[Export]
		private Control DeckNodesParent { get; set; }

		[Export]
		private PackedScene DeckBuilderInfoDisplayerPrefab { get; set; }
		[Export]
		private DeckBuilderBuiltDeckAvatarInfoDisplayer AvatarInfoDisplayer { get; set; }

		[Export]
		private DeckBuilderController DeckBuilderController { get; set; }

		[Export]
		private Control[] Tabs { get; set; }

		[Export]
		private OptionButton DeckNameSelect { get; set; }

		private readonly List<string> deckNames = new();

		private Decklist currentDeck;

		public override void _Ready()
		{
			EnsureDeckDirectory();
			using var folder = DirAccess.Open(DeckFolderPath);
			foreach (string deckFileName in folder.GetFiles())
			{
				if (deckFileName[^5..] != ".json") return; // GD.Print($"{deckFileName[^5..]}");
				string deckName = deckFileName[..^5];
				AddDeck(deckName);
			}

			if (deckNames.Count > 0) LoadDeck(deckNames[0]);
		}

		private void AddDeck(string deckName)
		{
			deckNames.Add(deckName);
			DeckNameSelect.AddItem(deckName);
		}

		private static void EnsureDeckDirectory()
		{
			if (!DirAccess.DirExistsAbsolute(DeckFolderPath))
			{
				using var folder = DirAccess.Open("user://");
				folder.MakeDir(DeckFolderPath);
			}
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
			AddDeck(name);
			DeckNameSelect.Select(DeckNameSelect.ItemCount - 1);
			SaveDeck();
		}

		private void ClearDeck()
		{
			foreach (var node in DeckNodesParent.GetChildren()) node.QueueFree();
			AvatarInfoDisplayer.Clear();
		}

		private void SaveDeck()
		{
			EnsureDeckDirectory();

			using var deck = FileAccess.Open($"{DeckFolderPath}/{currentDeck.deckName}.json", FileAccess.ModeFlags.Write);
			if (deck == null) GD.Print(FileAccess.GetOpenError());
			string json = JsonConvert.SerializeObject(currentDeck);
			deck.StoreString(json);
		}

		public void LoadDeck(int index) => LoadDeck(deckNames[index]);

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
				var avatar = DeckBuilderController.CardRepository.CreateDeckBuilderCard(decklist.avatarName);
				var avatarView = new DeckBuilderCardView(AvatarInfoDisplayer);
				var avatarCtrl = new DeckBuilderCardController(avatarView, avatar);
				avatarCtrl.Display();
			}

			if (decklist.deck != null)
				foreach (string cardName in decklist.deck) AddToDeck(cardName);

			currentDeck = decklist;
		}

		private void AddToDeck(string cardName)
		{
			var card = DeckBuilderController.CardRepository.CreateDeckBuilderCard(cardName);
			AddToDeck(card);
		}

		private void AddToDeck(DeckBuilderCard card)
		{
			var infoDisplayer = CreateDeckInfoDisplayer();
			var view = new DeckBuilderCardView(infoDisplayer);
			var ctrl = new DeckBuilderCardController(view, card);
			DeckNodesParent.AddChild(view.InfoDisplayer);
			ctrl.Display();
			currentDeck?.deck.Add(card.CardName); //It's ok that we add to the decklist before replacing it in LoadDeck because it just gets garbage collected
		}

		private DeckBuilderBuiltDeckInfoDisplayer CreateDeckInfoDisplayer()
		{
			if (DeckBuilderInfoDisplayerPrefab.Instantiate() is not DeckBuilderBuiltDeckInfoDisplayer card)
					throw new System.ArgumentNullException(nameof(DeckBuilderInfoDisplayerPrefab), "Was not the right type");
			return card;
		}
	}
}