using System.Collections;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Kompas.Shared
{
	public static class DeckAccess
	{
		public const string DeckFolderPath = "user://Decks";

		public static IList<string> GetDeckNames()
		{
			var deckNames = new List<string>();

			EnsureDeckDirectory();
			using var folder = DirAccess.Open(DeckFolderPath);
			foreach (string deckFileName in folder.GetFiles())
			{
				if (deckFileName[^5..] != ".json")
				{
					Logger.Err($"{deckFileName} is not a deck, but it was in the deck folder...");
					continue;
				} 
				string deckName = deckFileName[..^5];
				deckNames.Add(deckName);
			}

			return deckNames;
		}

		public static void Save(Decklist decklist)
		{
			EnsureDeckDirectory();

			using var deck = FileAccess.Open($"{DeckFolderPath}/{decklist.deckName}.json", FileAccess.ModeFlags.Write);
			if (deck == null)
			{
				Logger.Log(FileAccess.GetOpenError());
				return;
			}
			string json = JsonConvert.SerializeObject(decklist);
			deck.StoreString(json);
		}

		public static void Delete(Decklist decklist)
		{
			EnsureDeckDirectory();

			using var deckFolder = DirAccess.Open(DeckFolderPath);
			if (deckFolder == null)
			{
				Logger.Log(DirAccess.GetOpenError());
				return;
			}

			deckFolder.Remove($"{decklist.deckName}.json");
		}

		public static Decklist? Load(string deckName)
		{
			var path = $"{DeckFolderPath}/{deckName}.json";
			if (!FileAccess.FileExists(path)) return null;

			using var deck = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = deck.GetAsText();
			Logger.Log($"Loading {json}");
			return JsonConvert.DeserializeObject<Decklist>(json);
		}

		private static void EnsureDeckDirectory()
		{
			if (!DirAccess.DirExistsAbsolute(DeckFolderPath))
			{
				using var folder = DirAccess.Open("user://");
				folder.MakeDir(DeckFolderPath);
			}
		}
	}
}