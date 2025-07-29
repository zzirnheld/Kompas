using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.UI;

namespace Kompas.Client.Cards.Views;

public partial class ClientTopLeftCameraView : FocusableCardViewBase<ClientGameCard, ClientTopLeftCameraDisplayer>
{
	private ClientTopLeftCameraDisplayer TopLeftCamera { get; }

	public ClientTopLeftCameraView(ClientTopLeftCameraDisplayer topLeftCamera)
		: base(topLeftCamera)
	{
		TopLeftCamera = topLeftCamera;
	}

	protected override void Display(ClientGameCard shownCard)
	{
		TopLeftCamera.ShowingInfo = true;
		TopLeftCamera.Display(shownCard);
	}

	public new void Focus(ClientGameCard? card)
	{
		//TODO: also stats? or just location?
		//TODO: consider moving out this focus logic to something that isn't supposed to just be a view. it shouldn't be responsible for this, too
		// that way, that focus thing could just be responsible for knowing who we're focusing on, and the view can just be concerned with showing
		// That's kinda what the "view" class is doing here anyway, tho. might be more logical to document that, and add the calls to stats refreshed accordingly
		// so that we know to update stats in top left when stats update on card?
		// tho theoretically that should be handled by the camera displayer.
		// yeah, I think the role of this "role" class is largely to handle focus, and maybe also reminder text.
		// need to document that
		if (FocusedCard != null) FocusedCard.CardController.LocationRefreshed -= RefreshFocus;

		base.Focus(card);
		Refresh(); //To force updating layers

		if (card == null) return;
		card.CardController.LocationRefreshed += RefreshFocus;
	}
	public void Hover(ClientGameCard? card, bool refresh = false) => base.Show(card, refresh);

	//This is its own function, not a lambda, so it can unsubscribe.
	private void RefreshFocus(object? _, GameCard? card) => RefreshFocus(card);

	/// <summary>
	/// If <paramref name="card"/> is the shown card, refreshes its shown information
	/// </summary>
	private void RefreshFocus(GameCard? card)
	{
		Logger.Log($"Refreshing focus on {card?.CardName}#{FocusedCard?.ID} while focused on {FocusedCard?.CardName}#{FocusedCard?.ID}");
		if (card == FocusedCard && card != null) ShiftFocus(FocusedCard);
	}
}