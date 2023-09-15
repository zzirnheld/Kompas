using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kompas.Cards.Models;

namespace Kompas.Cards.Loading
{
	public abstract class CardRepository
	{
		public const string JsonsFolderPath = "res://Jsons";
		public const string CardJsonsFolderPath = $"{JsonsFolderPath}/Cards";
		public const string CardListFilePath = $"{CardJsonsFolderPath}/Card List.txt";

		public const string KeywordJsonsFolderPath = $"{JsonsFolderPath}/Keywords/Full";
		public const string keywordListFilePath = $"{KeywordJsonsFolderPath}/Keyword List";

		public const string PartialKeywordFolderPath = $"{JsonsFolderPath}/Keywords/Partial";
		public const string PartialKeywordListFilePath = $"{PartialKeywordFolderPath}/Keyword List";

		public const string TriggerKeywordFolderPath = $"{JsonsFolderPath}/Keywords/Trigger";
		public const string TriggerKeywordListFilePath = $"{TriggerKeywordFolderPath}/Keyword List";

		public static readonly string RemindersJsonPath = $"res://Reminder Text/Reminder Texts";

		private static readonly Regex subeffRegex = new(@"Subeffect:([^:]+):"); //Subeffect:*:
		private const string subeffReplacement = @"KompasServer.Effects.Subeffects.$1, Assembly-CSharp";

		//restriction regexes
		private static readonly Regex coreRestrictionRegex = new(@"Restrict\.([^:]+):([^:]+):"); //Restrict.*:*:
		private const string coreRestrictionReplacement = @"KompasCore.Effects.Restrictions.$1.$2, Assembly-CSharp";

		//identity regexes
		private static readonly Regex cardsIdentityRegex = new(@"""Cards:([^:]+):"); //"Cards:*:
		private const string cardsIdentityReplacement = @"""KompasCore.Effects.Identities.Cards.$1, Assembly-CSharp";

		private static readonly Regex manyCardsIdentityRegex = new(@"""ManyCards:([^:]+):"); //"ManyCards:*:
		private const string manyCardsIdentityReplacement = @"""KompasCore.Effects.Identities.ManyCards.$1, Assembly-CSharp";

		private static readonly Regex spacesIdentityRegex = new(@"""Spaces:([^:]+):"); //"Spaces:*:
		private const string spacesIdentityReplacement = @"""KompasCore.Effects.Identities.Spaces.$1, Assembly-CSharp";

		private static readonly Regex manySpacesIdentityRegex = new(@"""ManySpaces:([^:]+):"); //"ManySpaces:*:
		private const string manySpacesIdentityReplacement = @"""KompasCore.Effects.Identities.ManySpaces.$1, Assembly-CSharp";

		private static readonly Regex numbersIdentityRegex = new(@"""Numbers:([^:]+):"); //"Numbers:*:
		private const string numbersIdentityReplacement = @"""KompasCore.Effects.Identities.Numbers.$1, Assembly-CSharp";

		private static readonly Regex manyNumbersIdentityRegex = new(@"""ManyNumbers:([^:]+):"); //"ManyNumbers:*:
		private const string manyNumbersIdentityReplacement = @"""KompasCore.Effects.Identities.ManyNumbers.$1, Assembly-CSharp";

		private static readonly Regex playersIdentityRegex = new(@"""Players:([^:]+):"); //"Players:*:
		private const string playersIdentityReplacement = @"""KompasCore.Effects.Identities.Players.$1, Assembly-CSharp";

		private static readonly Regex stackablesIdentityRegex = new(@"""Stackables:([^:]+):"); //"Stackables:*:
		private const string stackablesIdentityReplacement = @"""KompasCore.Effects.Identities.Stackables.$1, Assembly-CSharp";

		//relationships
		private static readonly Regex relationshipRegex = new(@"Relationships\.([^:]+):([^:]+):"); //Relationships.*:*:
		private const string relationshipReplacement = @"KompasCore.Effects.Relationships.$1Relationships.$2, Assembly-CSharp";

		private static readonly Regex numberSelectorRegex = new(@"NumberSelector:([^:]+):"); //NumberSelector:*:
		private const string numberSelectorReplacement = @"KompasCore.Effects.Identities.NumberSelectors.$1, Assembly-CSharp";

		private static readonly Regex threeSpaceRelationshipRegex = new(@"ThreeSpaceRelationships:([^:]+):"); //ThreeSpaceRelationships:*:
		private const string threeSpaceRelationshipReplacement = @"KompasCore.Effects.Identities.ThreeSpaceRelationships.$1, Assembly-CSharp";

		protected static readonly JsonSerializerSettings CardLoadingSettings = new()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			MaxDepth = null,
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize
		};

		private static readonly string[] cardNamesToIgnore = new string[] { "Square Kompas Logo" };

		protected static readonly Dictionary<string, string> cardJsons = new();
		protected static readonly Dictionary<string, string> cardFileNames = new();
		public static IReadOnlyCollection<string> CardNames => cardJsons.Keys;

		protected static readonly Dictionary<string, string> keywordJsons = new();
		protected static readonly Dictionary<string, string> partialKeywordJsons = new();
		protected static readonly Dictionary<string, string> triggerKeywordJsons = new();

		public static ReminderTextsContainer Reminders { get; private set; }
		public static ICollection<string> Keywords { get; private set; }

		private static bool initalized = false;
		private static readonly object initializationLock = new();

		/*
		public Game game;
		public Settings Settings
		{
			get
			{
				if (game != null) return game.Settings;
				else return default;
			}
		}*/

		public static IEnumerable<string> CardJsons => cardJsons.Values;

		protected CardRepository()
		{
			InitializeCardJsons(); //TODO replace with what's currently in Awake()
		}

		protected virtual void Awake()
		{
			lock (initializationLock)
			{
				if (initalized) return;
				initalized = true;

				InitializeCardJsons();

				InitializeMapFromJsons(keywordListFilePath, KeywordJsonsFolderPath, keywordJsons);
				InitializeMapFromJsons(PartialKeywordListFilePath, PartialKeywordFolderPath, partialKeywordJsons);
				InitializeMapFromJsons(TriggerKeywordListFilePath, TriggerKeywordFolderPath, triggerKeywordJsons);

				var reminderJsonAsset = LoadFileAsText(RemindersJsonPath);
				//Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(reminderJsonAsset);
				//Reminders.Initialize();
				Keywords = Reminders.keywordReminderTexts.Select(rti => rti.keyword).ToArray();
			}
		}

		private static string LoadFileAsText(string path)
		{
			//GD.Print($"Trying to load {path}");
			if (!FileAccess.FileExists(path)) return null;

			using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

			return file.GetAsText();

			/*
			var json = ResourceLoader.Load<Json>(path);
			GD.Print($"{Json.Stringify(json)}\n\n{json.GetParsedText()}");
			Json.Stringify(json);
			return json.GetParsedText(); */
		}

		private static void InitializeCardJsons()
		{
			static bool isCardToIgnore(string name) => string.IsNullOrWhiteSpace(name) || cardNamesToIgnore.Contains(name);

			string cardFilenameList = LoadFileAsText(CardListFilePath);
			cardFilenameList = cardFilenameList.Replace('\r', '\n');
			string[] cardFilenameArray = cardFilenameList.Split('\n');

			foreach (string filename in cardFilenameArray)
			{
				if (string.IsNullOrEmpty(filename)) continue;
				//sanitize the filename. for some reason, doing substring fixes stuff
				string filenameClean = filename.Substring(0, filename.Length);
				//don't add duplicate cards
				if (isCardToIgnore(filenameClean) || CardExists(filenameClean)) continue;

				//load the json
				var jsonAsset = LoadFileAsText($"{CardJsonsFolderPath}/{filenameClean}.json");
				if (jsonAsset == null)
				{
					GD.PrintErr($"Failed to load json file for {filenameClean}");
					continue;
				}
				string json = jsonAsset;

				//handle tags like subeffs, etc.
				json = ReplacePlaceholders(json);

				//load the cleaned json to get the card's name according to itself
				SerializableCard card = SerializableCardFromJson(json);
				if (card == null) continue;

				string cardName = card.cardName;

				//add the cleaned json to the dictionary
				//if this throws a key existing exception, you probably have two cards with the same name field, but diff file names
				if (cardJsons.ContainsKey(cardName)) continue;
				cardJsons.Add(cardName, json);
				cardFileNames.Add(cardName, filename);
			}

			GD.Print(string.Join("\n", CardNames));
		}

		protected static SerializableCard SerializableCardFromJson(string json)
		{
			try
			{
				return JsonConvert.DeserializeObject<SerializableCard>(json, CardLoadingSettings);
			}
			catch (JsonException e)
			{
				GD.PrintErr($"Failed to load {json}. Error\n{e}");
				return null;
			}
		}

		private static void InitializeMapFromJsons(string filePath, string folderPath, Dictionary<string, string> dict)
		{
			string keywordList = LoadFileAsText(filePath);
			var keywords = keywordList.Replace('\r', '\n').Split('\n').Where(s => !string.IsNullOrEmpty(s));
			GD.Print($"Keywords list: \n{string.Join("\n", keywords.Select(keyword => $"{keyword} length {keyword.Length}"))}");
			foreach (string keyword in keywords)
			{
				GD.Print($"Loading {keyword} from {folderPath}/{keyword}");
				string json = LoadFileAsText($"{folderPath}/{keyword}");
				json = ReplacePlaceholders(json);
				dict.Add(keyword, json);
			}
		}

		private static string ReplacePlaceholders(string json)
		{
			//remove problematic chars for from json function
			json = json.Replace('\n', ' ');
			json = json.Replace("\r", "");
			json = json.Replace("\t", "");

			json = subeffRegex.Replace(json, subeffReplacement);

			json = coreRestrictionRegex.Replace(json, coreRestrictionReplacement);

			//Many before single, to not replace the many with a broken thing
			json = manyCardsIdentityRegex.Replace(json, manyCardsIdentityReplacement);
			json = cardsIdentityRegex.Replace(json, cardsIdentityReplacement);

			json = manySpacesIdentityRegex.Replace(json, manySpacesIdentityReplacement);
			json = spacesIdentityRegex.Replace(json, spacesIdentityReplacement);

			json = manyNumbersIdentityRegex.Replace(json, manyNumbersIdentityReplacement);
			json = numbersIdentityRegex.Replace(json, numbersIdentityReplacement);

			json = playersIdentityRegex.Replace(json, playersIdentityReplacement);
			json = stackablesIdentityRegex.Replace(json, stackablesIdentityReplacement);

			json = relationshipRegex.Replace(json, relationshipReplacement);
			json = numberSelectorRegex.Replace(json, numberSelectorReplacement);
			json = threeSpaceRelationshipRegex.Replace(json, threeSpaceRelationshipReplacement);

			return json;
		}

		public static bool CardExists(string cardName) => CardNames.Contains(cardName);

		public static string GetJsonFromName(string name)
		{
			if (!cardJsons.ContainsKey(name))
			{
				//This log exists exclusively for debugging purposes
				GD.PrintErr($"No json found for name \"{name ?? "null"}\" of length {name?.Length ?? 0}");
				return null;
			}

			return cardJsons[name];
		}

		public static IEnumerable<string> GetJsonsFromNames(IEnumerable<string> names)
			=> names.Select(n => GetJsonFromName(n)).Where(json => json != null);

		public static string FileNameFor(string cardName) => cardFileNames[cardName];

		public static Texture2D LoadSprite(string cardFileName) => GD.Load<Texture2D>($"Card Face Images/{cardFileName}");

		public static IEnumerable<SerializableCard> SerializableCards => cardJsons.Values.Select(SerializableCardFromJson).Where(c => c != null);
	}
}