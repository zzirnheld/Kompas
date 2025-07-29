using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Cards.Loading;

public interface ICardRepository
{
	public string? FileNameFor(string? cardName);
	public string? GetJsonFromName(string? cardName);

	public (string fieldText, string elseText, IList<ReminderTextInfo> replacedKeywords)
		Enhance(string cardEffText, IReadOnlyCollection<IEffect> effects);
	public ReminderTextInfo LookupKeywordReminderText(string keyword);

	public Texture2D? LoadSprite(string cardFileName);
}

public interface IFileLoader
{
	public static IFileLoader Godot => new FileLoader();

	public string? LoadFileAsText(string path);

	public Texture2D? LoadSprite(string cardFileName);

	private class FileLoader : IFileLoader
	{
		public string? LoadFileAsText(string path)
		{
			//Logger.Log($"Trying to load {path}");
			if (!FileAccess.FileExists(path)) return null;

			using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

			return file.GetAsText();

			/*
			var json = ResourceLoader.Load<Json>(path);
			Logger.Log($"{Json.Stringify(json)}\n\n{json.GetParsedText()}");
			Json.Stringify(json);
			return json.GetParsedText(); */
		}

		public Texture2D? LoadSprite(string cardFileName)
		{
			string path = $"{CardRepository.CardImagesPath}/{cardFileName}.png";
			if (!ResourceLoader.Exists(path))
			{
				Logger.Log($"Warning: texture not found at {cardFileName}");
				return null;
			}
			else return ResourceLoader.Load<Texture2D>(path);
		}
	}
}

public abstract class CardRepository : ICardRepository
{
	private const string CharCardFramePath = "res://Icons/Card Stuff/Char Frame.svg";
	private const string NonCharCardFramePath = "res://Icons/Card Stuff/NonChar Frame.svg";

	public const string JsonsFolderPath = "res://Jsons";
	public const string CardJsonsFolderPath = $"{JsonsFolderPath}/Cards";
	public const string CardListFilePath = $"{CardJsonsFolderPath}/Card List.txt";

	public const string KeywordJsonsFolderPath = $"{JsonsFolderPath}/Keywords/Full";
	public const string keywordListFilePath = $"{KeywordJsonsFolderPath}/Keyword List.txt";

	public const string PartialKeywordFolderPath = $"{JsonsFolderPath}/Keywords/Partial";
	public const string PartialKeywordListFilePath = $"{PartialKeywordFolderPath}/Keyword List.txt";

	public const string TriggerKeywordFolderPath = $"{JsonsFolderPath}/Keywords/Trigger";
	public const string TriggerKeywordListFilePath = $"{TriggerKeywordFolderPath}/Keyword List.txt";

	public static readonly string RemindersJsonPath = $"res://Jsons/Reminder Texts.json";
	public static readonly string CardImagesPath = "res://Sprites";

	private static readonly Regex subeffRegex = new(@"Subeffect:([^:]+):"); //Subeffect:*:
	private const string subeffReplacement = @"Kompas.Server.Effects.Models.Subeffects.$1, Kompas";

	private static readonly Regex coreRestrictionRegex = new(@"Restrict\.([^:]+):([^:]+):"); //Restrict.*:*:
	private const string coreRestrictionReplacement = @"Kompas.Effects.Models.Restrictions.$1.$2, Kompas";

	private static readonly Regex coreIdentityRegex = new(@"Identify\.([^:]+):([^:]+):"); //Restrict.*:*:
	private const string coreIdentityReplacement = @"Kompas.Effects.Models.Identities.$1.$2, Kompas";

	private static readonly Regex relationshipRegex = new(@"Relationships\.([^:]+):([^:]+):"); //Relationships.*:*:
	private const string relationshipReplacement = @"Kompas.Effects.Models.Relationships.$1.$2, Kompas";

	private static readonly Regex numberSelectorRegex = new(@"Selectors.([^:]+):([^:]+):"); //NumberSelector:*:
	private const string numberSelectorReplacement = @"Kompas.Effects.Models.Selectors.$1.$2, Kompas";

	private static readonly Regex usesRegex = new(@"\[USES.([^\.]+).([^\]]+)\]");

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

	private static ReminderTextsContainer? _reminders;
	public static ReminderTextsContainer Reminders
	{
		get => _reminders ?? throw new NotInitializedException();
		set => _reminders = value;
	}
	private static bool initalized = false;
	public static bool Initialized => initalized;
	private static readonly object initializationLock = new();

	private static Texture2D? _charCardFrameTexture;
	public static Texture2D CharCardFrameTexture => _charCardFrameTexture ??= ResourceLoader.Load<Texture2D>(CharCardFramePath);

	private static Texture2D? _noncharCardFrameTexture;
	public static Texture2D NoncharCardFrameTexture => _noncharCardFrameTexture ??= ResourceLoader.Load<Texture2D>(NonCharCardFramePath);

	private readonly IFileLoader fileLoader;
	protected readonly bool throwExceptions;
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

	protected CardRepository(IFileLoader fileLoader, bool throwExceptions)
	{
		this.fileLoader = fileLoader;
		this.throwExceptions = throwExceptions;
	}

	public Texture2D? LoadSprite(string cardFileName) => fileLoader.LoadSprite(cardFileName);

	// Ideally, should be called as each card repository gets created.
	// I don't think it's ok for this to be in the constructor.
	protected void Initialize()
	{
		lock (initializationLock)
		{
			if (initalized) return;

			InitializeCardJsons();

			InitializeMapFromJsons(keywordListFilePath, KeywordJsonsFolderPath, keywordJsons);
			InitializeMapFromJsons(PartialKeywordListFilePath, PartialKeywordFolderPath, partialKeywordJsons);
			InitializeMapFromJsons(TriggerKeywordListFilePath, TriggerKeywordFolderPath, triggerKeywordJsons);

			var reminderJsonAsset = fileLoader.LoadFileAsText(RemindersJsonPath)
				?? throw new System.NullReferenceException("Failed to load reminders json");
			Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(reminderJsonAsset)
				?? throw new System.NullReferenceException("Failed to load reminder texts from the json");
			Reminders.Initialize();
			initalized = true;
		}
	}

	private void InitializeCardJsons()
	{
		static bool isCardToIgnore(string name) => string.IsNullOrWhiteSpace(name) || cardNamesToIgnore.Contains(name);

		string? cardFilenameList = fileLoader.LoadFileAsText(CardListFilePath)
			?? throw new System.NullReferenceException("Failed to load card list");
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
			var jsonAsset = fileLoader.LoadFileAsText($"{CardJsonsFolderPath}/{filenameClean}.json");
			if (jsonAsset == null)
			{
				Logger.Err($"Failed to load json file for {filenameClean}");
				continue;
			}
			string json = jsonAsset;

			//handle tags like subeffs, etc.
			json = ReplacePlaceholders(json);

			//load the cleaned json to get the card's name according to itself
			var card = SerializableCardFromJson(json);
			if (card == null) continue;

			string cardName = card.cardName
				?? throw new System.NullReferenceException("Card had a null name!");

			//add the cleaned json to the dictionary
			//if this throws a key existing exception, you probably have two cards with the same name field, but diff file names
			if (cardJsons.ContainsKey(cardName)) continue;
			cardJsons.Add(cardName, json);
			cardFileNames.Add(cardName, filename);
		}

		//Logger.Log(string.Join(", ", CardNames));
	}

	protected static SerializableCard? SerializableCardFromJson(string json)
	{
		try
		{
			return JsonConvert.DeserializeObject<SerializableCard>(json, CardLoadingSettings);
		}
		catch (JsonException e)
		{
			Logger.Err($"Failed to load {json}. Error\n{e}");
			return null;
		}
	}

	private void InitializeMapFromJsons(string filePath, string folderPath, Dictionary<string, string> dict)
	{
		string file = fileLoader.LoadFileAsText(filePath)
			?? throw new System.NullReferenceException($"Failed to load {filePath}");
		var lines = file.Replace('\r', '\n')
			.Split('\n')
			.Where(s => !string.IsNullOrEmpty(s));
		Logger.Log($"Keywords list: \n{string.Join("\n", lines.Select(line => $"{line} length {line.Length}"))}");
		foreach (string line in lines)
		{
			Logger.Log($"Loading {line} from {folderPath}/{line}");
			string json = fileLoader.LoadFileAsText($"{folderPath}/{line}.json")
				?? throw new System.NullReferenceException($"Failed to load {line}");
			json = ReplacePlaceholders(json);
			dict.Add(line, json);
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
		json = coreIdentityRegex.Replace(json, coreIdentityReplacement);

		json = relationshipRegex.Replace(json, relationshipReplacement);
		json = numberSelectorRegex.Replace(json, numberSelectorReplacement);

		return json;
	}

	public static bool CardExists(string? cardName) => CardNames.Contains(cardName);

	public string? GetJsonFromName(string? name)
	{
		if (name == null || !cardJsons.ContainsKey(name))
		{
			//This log exists exclusively for debugging purposes
			Logger.Err($"No json found for name \"{name ?? "null"}\" of length {name?.Length ?? 0}");
			return null;
		}

		return cardJsons[name];
	}

	public IEnumerable<string> GetJsonsFromNames(IEnumerable<string> names)
		=> names
			.Select(n => GetJsonFromName(n))
			.NonNull();

	public string? FileNameFor(string? cardName)
	{
		if (cardName == null) return null;
		else return cardFileNames[cardName];
	}

	public static IEnumerable<SerializableCard> SerializableCards
		=> cardJsons.Values
			.Select(SerializableCardFromJson)
			.NonNull();

	public static ITriggerRestriction[]? InstantiateTriggerKeyword(string keyword)
	{
		if (!triggerKeywordJsons.ContainsKey(keyword))
		{
			Logger.Err($"No trigger keyword json found for {keyword}");
			return System.Array.Empty<ITriggerRestriction>();
		}
		try
		{
			return JsonConvert.DeserializeObject<ITriggerRestriction[]>
				(triggerKeywordJsons[keyword], CardLoadingSettings);
		}
		catch (JsonReaderException)
		{
			Logger.Err($"Failed to instantiate {keyword}");
			throw;
		}
	}

	public (string fieldText, string elseText, IList<ReminderTextInfo> replacedKeywords)
		Enhance(string cardEffText, IReadOnlyCollection<IEffect> effects)
	{
		var (withKeywordsReplaced, replacedKeywords) = HandleKeywords(cardEffText);
		string replacedWithFallback = usesRegex.Replace(withKeywordsReplaced, match => UseToText(match, effects, true));
		string replacedWithoutFallback = usesRegex.Replace(withKeywordsReplaced, match => UseToText(match, effects, false));
		return (replacedWithoutFallback, replacedWithFallback, replacedKeywords);
	}

	private static string UseToText(Match match, IReadOnlyCollection<IEffect> effects, bool fallBack)
	{
		if (match.Groups.Count < 3)
		{
			Logger.Warn("Somehow a use string only had 1 match group!");
			return string.Empty;
		}
		if (fallBack) return match.Groups[2].Value;

		if (!int.TryParse(match.Groups[1].Value, out int effIndex))
		{
			Logger.Err($"Uses argument {match.Groups[1].Value} was not an integer!");
			return string.Empty;
		}

		var eff = effects.ElementAtOrDefault(effIndex);
		if (eff == null)
		{
			//TODO don't print this error if effects is purposefully an empty array, for deck builder, for example.
			//make this a virtual method?
			Logger.Err($"Uses index {match.Groups[1].Value} was not within the bounds of the effects array ({effects.Count})!");
			return string.Empty;
		}

		string grey = "#a0a0a0";

		//FUTURE: revisit if I add effects with max per stack AND per turn. but that's probably too confusing anyway
		int? perTurn = eff.MaxPerTurn();
		if (perTurn != null) return $"{match.Groups[2].Value} [color={grey}]({eff.TimesUsedThisTurn}/{perTurn})[/color]";

		int? perRound = eff.MaxPerRound();
		if (perRound != null) return $"{match.Groups[2].Value} [color={grey}]({eff.TimesUsedThisRound}/{perRound})[/color]";

		int? perStack = eff.MaxPerStack();
		if (perStack != null) return $"{match.Groups[2].Value} [color={grey}]({eff.TimesUsedThisStack}/{perStack})[/color]";

		return string.Empty;
	}

	/// <summary>
	/// Adds BBCode [hint] tags for keyword reminders.
	/// </summary>
	/// <param name="baseEffText"></param>
	/// <returns></returns>
	public (string, IList<ReminderTextInfo>) HandleKeywords(string baseEffText)
	{
		string bbCodeEffText = baseEffText;
		var list = new List<ReminderTextInfo>();
		foreach (var reminderTextInfo in Reminders.KeywordToReminder.Values)
		{
			if (!reminderTextInfo.KeywordReplaceRegex.IsMatch(bbCodeEffText)) continue;

			list.Add(reminderTextInfo);
			string keywordTag = ConstructKeywordTag(reminderTextInfo);
			bbCodeEffText = reminderTextInfo.KeywordReplaceRegex.Replace(bbCodeEffText, keywordTag);
		}
		return (bbCodeEffText, list);
	}

	protected virtual string ConstructKeywordTag(ReminderTextInfo reminderTextInfo) => $"[u]{reminderTextInfo.Keyword}[/u]";

	public ReminderTextInfo LookupKeywordReminderText(string keyword) => Reminders.KeywordToReminder[keyword];
}