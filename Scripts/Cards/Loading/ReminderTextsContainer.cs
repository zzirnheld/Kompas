using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Godot;
using Newtonsoft.Json;

namespace Kompas.Cards.Loading
{
	[DataContract]
	public class ReminderTextsContainer
	{
		[JsonProperty(Required = Required.Always)]
		public ReminderTextInfo[] keywordReminderTexts = Array.Empty<ReminderTextInfo>();

		public Dictionary<string, ReminderTextInfo> KeywordToReminder { get; set; } = new();

		public ReminderTextsContainer() { }

		public void Initialize()
		{
			foreach(var rti in keywordReminderTexts)
			{
				KeywordToReminder.Add(rti.KeywordStringKey, rti);
			}
		}
	}

	[DataContract]
	public class ReminderTextInfo
	{
		[JsonProperty]
		public string? keywordRegex;

		#pragma warning disable IDE0044 // Add readonly modifier
		[JsonProperty(Required = Required.Always)]
		private string? keyword;
		[JsonProperty(Required = Required.Always)]
		private string? reminder;
		#pragma warning restore IDE0044 // Add readonly modifier

		public string Keyword => keyword ?? throw new NullReferenceException(nameof(keyword));
		public string Reminder => reminder ?? throw new NullReferenceException(nameof(reminder));

		public string KeywordStringKey => keywordRegex ?? Keyword;
		private Regex? _keywordReplaceRegex;
		public Regex KeywordReplaceRegex => _keywordReplaceRegex ??= new(keywordRegex ?? Keyword);
	}
}