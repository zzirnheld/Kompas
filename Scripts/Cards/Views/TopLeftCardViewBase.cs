using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	public abstract class TopLeftCardViewBase<CardType> : FocusableCardViewBase<CardType, ControlInfoDisplayer>
		where CardType : CardBase
	{
		private ReminderTextPopup ReminderTextPopup { get; }
		
		protected abstract CardRepository CardRepository { get; }

		protected TopLeftCardViewBase(ControlInfoDisplayer infoDisplayer, ReminderTextPopup reminderTextPopup)
			: base(infoDisplayer)
		{
			ReminderTextPopup = reminderTextPopup;
			infoDisplayer.HoverKeyword += (_, keyword) => HoverReminderText(keyword);
			infoDisplayer.StopHoverKeyword += (_, keyword) => ReminderTextPopup.StopDisplaying();
		}

		private void HoverReminderText(string keyword)
		{
			if (ShownCard == null)
			{
				GD.PushWarning($"Somehow hovered over keyword {keyword} while shown card was null... ignoring.");
				return;
			}

			var reminderText = CardRepository.LookupKeywordReminderText(keyword);
			ReminderTextPopup.Display(reminderText);
		}
	}
}