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

	private readonly bool illusory;

	public ClientCardView(Zoomable3DCardInfoDisplayer infoDisplayer, ClientGameCard card, bool illusory)
		: base(infoDisplayer)
	{
		this.illusory = illusory;
		Focus(card);
	}

	protected override void Display(ClientGameCard shownCard)
	{
		base.Display(shownCard);

		InfoDisplayer.DisplayZoomed(zoomedIn: illusory);
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
		//Can replace the below with Searching(shownCard.Location, shownCard.ControllingPlayerIndex == 0)
		//in order to only grey out things in the same area (potentially a setting?), but if you do, //TODO again, a better def for friendly
		bool greyout = targetingController.Searching()
			&& !targetingController.IsValidTarget(shownCard);
		InfoDisplayer.DisplayGreyedOut(greyout);

		InfoDisplayer.DisplayUnselectedValidTarget(targetingController.IsUnselectedValidTarget(shownCard));
		InfoDisplayer.DisplayCurrentTarget(targetingController.IsSelectedTarget(shownCard));
	}
}