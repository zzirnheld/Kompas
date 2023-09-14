using System.Collections.Generic;

namespace Kompas.Cards.Loading
{
	public class ReminderTextsContainer
	{
		public ReminderTextInfo[] keywordReminderTexts;

		public Dictionary<string, string> KeywordToReminder { get; } = new();

		public void Initialize()
		{
			foreach(var rti in keywordReminderTexts)
			{
				KeywordToReminder.Add(rti.keyword, rti.reminder);
			}
		}
	}

	public class ReminderTextInfo
	{
		public string keyword;
		public string reminder;
	}
}