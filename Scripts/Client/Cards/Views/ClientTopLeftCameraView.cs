using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.UI;

namespace Kompas.Client.Cards.Views;

public partial class ClientTopLeftCameraView : FocusableCardViewBase<ClientGameCard, ClientTopLeftCameraDisplayer>
{
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
		TopLeftCamera.ShowingInfo = true;
		TopLeftCamera.Display(shownCard);	
	}

	public new void Focus(ClientGameCard? card)
	{
		base.Focus(card);
		Refresh(); //To force updating layers
	} 
	public void Hover(ClientGameCard? card, bool refresh = false) => base.Show(card, refresh);
}