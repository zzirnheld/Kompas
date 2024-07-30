using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.UI;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views;

public partial class ClientTopLeftCameraView : FocusableCardViewBase<ClientGameCard, ClientTopLeftCameraDisplayer>

{
	//TODO: when card becomes selected, move camera to see it (should move with card so it's not circling while the card is oscillating)
	//TODO, when no card selected, hide texture

	private ReminderTextPopup ReminderTextPopup { get; }
	private ClientTopLeftCameraDisplayer TopLeftCamera { get; }

	private ICardRepository CardRepository => ShownCard?.Game.CardRepository
		?? throw new System.InvalidOperationException("Can't access a card repository while not showing cards!");

	public ClientTopLeftCameraView(ReminderTextPopup reminderTextPopup, ClientTopLeftCameraDisplayer topLeftCamera)
		: base(topLeftCamera)
	{
		TopLeftCamera = topLeftCamera;

		ReminderTextPopup = reminderTextPopup;
		topLeftCamera.HoverKeyword += (_, keyword) => HoverReminderText(keyword);
		topLeftCamera.StopHoverKeyword += (_, keyword) => ReminderTextPopup.StopDisplaying();
	}

	private void HoverReminderText(string keyword)
	{
		if (ShownCard == null)
		{
			Logger.Warn($"Somehow hovered over keyword {keyword} while shown card was null... ignoring.");
			return;
		}

		var reminderText = CardRepository.LookupKeywordReminderText(keyword);
		ReminderTextPopup.Display(reminderText);
	}

	protected override void Display(ClientGameCard shownCard)
	{
		
	}
}