using System;
using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.UI;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views
{
	public class ClientTopLeftCardView : FocusableCardViewBase<ClientGameCard, ControlInfoDisplayer>
	{
		private ReminderTextPopup ReminderTextPopup { get; }

		public ClientTopLeftCardView(ControlInfoDisplayer infoDisplayer, ReminderTextPopup reminderTextPopup)
			: base(infoDisplayer)
		{
			ReminderTextPopup = reminderTextPopup;
			infoDisplayer.HoverKeyword += (_, keyword) => HoverReminderText(keyword);
			infoDisplayer.StopHoverKeyword += (_, keyword) => ReminderTextPopup.StopDisplaying();
		}
		
		public void Select(ClientGameCard? card) => base.Focus(card);
		public void Hover(ClientGameCard? card, bool refresh = false) => base.Show(card, refresh);

		protected override void Show(ClientGameCard? card, bool refresh = false)
		{
			base.Show(card, refresh);

			if (card == null) return;
			card.CardController.AnythingRefreshed += (_, card) => Refresh(card);
		}

		/// <summary>
		/// If <paramref name="card"/> is the shown card, refreshes its shown information
		/// </summary>
		private void Refresh(GameCard? card)
		{
			if (card == ShownCard && card != null) Refresh();
		}

		private void HoverReminderText(string keyword)
		{
			if (ShownCard == null)
			{
				GD.PushWarning($"Somehow hovered over keyword {keyword} while shown card was null... ignoring.");
				return;
			}

			var reminderText = ShownCard.Game.CardRepository.LookupKeywordReminderText(keyword);
			ReminderTextPopup.Display(keyword, reminderText);
		}
	}
}