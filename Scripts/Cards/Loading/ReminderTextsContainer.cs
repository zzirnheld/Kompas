using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Kompas.Cards.Loading
{
	[DataContract]
	public class ReminderTextsContainer
	{
		[JsonProperty(Required = Required.Always)]
		public ReminderTextInfo[] keywordReminderTexts = Array.Empty<ReminderTextInfo>();

		public Dictionary<string, string> KeywordToReminder { get; set; } = new();

		public ReminderTextsContainer() { }

		public void Initialize()
		{
			foreach(var rti in keywordReminderTexts)
			{
				KeywordToReminder.Add(rti.keyword, rti.reminder);
			}
		}
	}

	[DataContract]
	public struct ReminderTextInfo
	{
		[JsonProperty(Required = Required.Always)]
		public string keyword;
		[JsonProperty(Required = Required.Always)]
		public string reminder;
	}
}