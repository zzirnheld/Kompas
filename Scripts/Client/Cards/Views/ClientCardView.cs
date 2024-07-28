using System;
using Godot;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Cards.Views;

public class ClientCardView : FocusableCardViewBase<ClientGameCard, Zoomable3DCardInfoDisplayer>
{
	//TODO get from settings instead, but since we're using properties I can just redirect the properties.
	private Color FriendlyColor => new(225f / 255f, 150f / 255f, 50f / 255f);
	private Color EnemyColor => new(188f / 255f, 188f / 255f, 188f / 255f);

	public ClientCardView(Zoomable3DCardInfoDisplayer infoDisplayer, ClientGameCard card)
		: base(infoDisplayer)
	{
		Focus(card);
	}

	protected override void Display(ClientGameCard shownCard)
	{
		base.Display(shownCard);

		InfoDisplayer.DisplayZoomed(zoomedIn: false); //For now, assume never zoomed in.
		DisplayFrame();

		DisplayTargeting(shownCard);
	}

	public void SetGreyedOut(bool greyedOut)
	{
		InfoDisplayer.DisplayGreyedOut(greyedOut);
	}

	private void DisplayFrame()
	{
		var color = ShownCard?.OwningPlayer.Friendly ?? false
			? FriendlyColor
			: EnemyColor;
		InfoDisplayer.DisplayFrame(color);
	}

	private void DisplayTargeting(ClientGameCard shownCard)
	{
		var targetingController = shownCard.ClientGame.ClientGameController.TargetingController;

		//Logger.Log($"{shownCard.CardName} is {(targetingController.IsValidTarget(shownCard) ? "" : "NOT ")}a valid target!");
		bool greyout = targetingController.Searching(shownCard.Location, shownCard.ControllingPlayerIndex == 0) //TODO again, a better def for friendly
			&& !targetingController.IsValidTarget(shownCard);
		InfoDisplayer.DisplayGreyedOut(greyout);

		InfoDisplayer.DisplayUnselectedValidTarget(targetingController.IsUnselectedValidTarget(shownCard));
		InfoDisplayer.DisplayCurrentTarget(targetingController.IsSelectedTarget(shownCard));
	}
}