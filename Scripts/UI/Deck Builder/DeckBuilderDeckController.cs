using System;
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

		private Decklist currentDeck;

		public override void _Ready()
		{
			//TEMP! create deck
			currentDeck = new()
			{
				deckName = "TEST",
				avatarName = "Animancer",
				deck = new System.Collections.Generic.List<string>{
					"Earth Golem",
					"Strength Core"
				}
			};
			SaveDeck();

			ClearDeck();

			LoadDeck("TEST");
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
		}

		private void ClearDeck()
		{
			foreach (var node in DeckNodesParent.GetChildren()) node.QueueFree();
			AvatarInfoDisplayer.Clear();
		}

		private void SaveDeck()
		{
			if (!DirAccess.DirExistsAbsolute(DeckFolderPath))
			{
				var folder = DirAccess.Open("user://");
				folder.MakeDir(DeckFolderPath);
			}

			using var deck = FileAccess.Open($"{DeckFolderPath}/{currentDeck.deckName}.json", FileAccess.ModeFlags.Write);
			if (deck == null) GD.Print(FileAccess.GetOpenError());
			string json = JsonConvert.SerializeObject(currentDeck);
			deck.StoreString(json);
		}

		private void LoadDeck(string deckName)
		{
			var path = $"{DeckFolderPath}/{deckName}.json";
			if (!FileAccess.FileExists(path)) return;

			using var deck = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = deck.GetAsText();
			Decklist decklist = JsonConvert.DeserializeObject<Decklist>(json);

			var avatar = DeckBuilderController.CardRepository.CreateDeckBuilderCard(decklist.avatarName);
			var avatarView = new DeckBuilderCardView(AvatarInfoDisplayer);
			var avatarCtrl = new DeckBuilderCardController(avatarView, avatar);
			avatarCtrl.Display();

			foreach (string cardName in decklist.deck)
			{
				var card = DeckBuilderController.CardRepository.CreateDeckBuilderCard(cardName);
				var infoDisplayer = CreateDeckInfoDisplayer();
				var view = new DeckBuilderCardView(infoDisplayer);
				var ctrl = new DeckBuilderCardController(view, card);
				DeckNodesParent.AddChild(view.InfoDisplayer);
				ctrl.Display();
			}

			currentDeck = decklist;
		}

		private DeckBuilderBuiltDeckInfoDisplayer CreateDeckInfoDisplayer()
		{
			if (DeckBuilderInfoDisplayerPrefab.Instantiate() is not DeckBuilderBuiltDeckInfoDisplayer card)
					throw new System.ArgumentNullException(nameof(DeckBuilderInfoDisplayerPrefab), "Was not the right type");
			return card;
		}
	}
}