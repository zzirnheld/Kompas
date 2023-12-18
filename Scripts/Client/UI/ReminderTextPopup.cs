using Godot;
using Kompas.Cards.Loading;
using Kompas.Shared.Exceptions;
using System;

namespace Kompas.Client.UI
{
	public partial class ReminderTextPopup : Control
	{
		[Export]
		private Label? _keyword;
		private Label Keyword => _keyword
			?? throw new UnassignedReferenceException();

		[Export]
		private Label? _reminderText;
		private Label ReminderText => _reminderText
			?? throw new UnassignedReferenceException();

		public void Display(ReminderTextInfo reminderTextInfo)
		{
			Keyword.Text = reminderTextInfo.keyword;
			ReminderText.Text = reminderTextInfo.reminder;
			var mousePos = GetViewport().GetMousePosition();
			OffsetLeft = OffsetRight = mousePos.X;
			OffsetTop = OffsetBottom = mousePos.Y;
			Visible = true;
		}

		public void StopDisplaying()
		{
			Visible = false;
		}
	}
}