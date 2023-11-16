using System;
using System.Collections.Generic;

namespace Kompas.Cards.Loading
{
	public struct ReminderTextsContainer
	{
		public ReminderTextInfo[] keywordReminderTexts = Array.Empty<ReminderTextInfo>();

		public Dictionary<string, string> KeywordToReminder { get; } = new();

		public ReminderTextsContainer() { }

		public readonly void Initialize()
		{
			foreach(var rti in keywordReminderTexts)
			{
				KeywordToReminder.Add(rti.keyword, rti.reminder);
			}
		}
	}

	public struct ReminderTextInfo
	{
		public string keyword;
		public string reminder;
	}
}