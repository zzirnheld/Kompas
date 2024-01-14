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
		
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public string keyword;
		[JsonProperty(Required = Required.Always)]
		public string reminder;
		#nullable restore

		public string KeywordStringKey => keywordRegex ?? keyword;
		private Regex? _keywordReplaceRegex;
		public Regex KeywordReplaceRegex => _keywordReplaceRegex ??= new(keywordRegex ?? keyword);
	}
}