using Godot;
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

		public void Display(string keyword, string reminderText)
		{
			Keyword.Text = keyword;
			ReminderText.Text = reminderText;
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