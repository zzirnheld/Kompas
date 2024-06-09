using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Kompas.Shared
{
	public class DeckAccess
	{
		private const string DotJson = ".json";
		private const string UserDataPrefix = "user://";
		private const string DeckFolderPath = $"{UserDataPrefix}Decks";

		public IReadOnlyCollection<string> DeckNames { get; init; }

		private IDictionary<string, Decklist> Decklists { get; } = new Dictionary<string, Decklist>();

		private DeckAccess()
		{
			DeckNames = System.Array.Empty<string>();
		}

		public static DeckAccess Create()
		{
			return new DeckAccess()
			{
				DeckNames = GetDeckNames(),
			};
		}

		private static IReadOnlyCollection<string> GetDeckNames()
		{
			var deckNames = new List<string>();

			EnsureDeckDirectory();
			using var folder = DirAccess.Open(DeckFolderPath);
			foreach (string deckFileName in folder.GetFiles())
			{
				if (deckFileName[^DotJson.Length..] != DotJson)
				{
					Logger.Err($"{deckFileName} is not a json, but it was in the deck folder...");
					continue;
				} 
				string deckName = deckFileName[..^DotJson.Length];
				deckNames.Add(deckName);
			}

			return deckNames;
		}

		public void Save(Decklist decklist)
		{
			if (string.IsNullOrEmpty(decklist.deckName)) return;

			EnsureDeckDirectory();

			Decklists[decklist.deckName] = decklist;

			using var deck = FileAccess.Open($"{DeckFolderPath}/{decklist.deckName}.json", FileAccess.ModeFlags.Write);
			if (deck == null)
			{
				Logger.Err(FileAccess.GetOpenError());
				return;
			}
			string json = JsonConvert.SerializeObject(decklist);
			deck.StoreString(json);
		}

		public void Delete(Decklist decklist)
		{
			if (string.IsNullOrEmpty(decklist.deckName)) return;

			EnsureDeckDirectory();

			Decklists.Remove(decklist.deckName);

			using var deckFolder = DirAccess.Open(DeckFolderPath);
			if (deckFolder == null)
			{
				Logger.Err(DirAccess.GetOpenError());
				return;
			}

			deckFolder.Remove($"{decklist.deckName}.json");
		}

		public Decklist? Load(string deckName)
		{
			if (string.IsNullOrEmpty(deckName)) return null;

			if (Decklists.ContainsKey(deckName)) return Decklists[deckName];

			var path = $"{DeckFolderPath}/{deckName}.json";
			if (!FileAccess.FileExists(path)) return null;

			using var deck = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = deck.GetAsText();
			Logger.Log($"Loading {json}");

			var ret = JsonConvert.DeserializeObject<Decklist>(json);
			if (ret != null) Decklists[deckName] = ret;
			return ret;
		}

		private static void EnsureDeckDirectory()
		{
			if (!DirAccess.DirExistsAbsolute(DeckFolderPath))
			{
				using var folder = DirAccess.Open(UserDataPrefix);
				folder.MakeDir(DeckFolderPath);
			}
		}
	}
}