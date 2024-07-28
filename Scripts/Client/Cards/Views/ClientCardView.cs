using Godot;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;

namespace Kompas.Client.Cards.Views;

public class ClientCardView : FocusableCardViewBase<ClientGameCard, Zoomable3DCardInfoDisplayer>
{
	private bool greyedOut;
	private Vector4 frameColor;

	public ClientCardView(Zoomable3DCardInfoDisplayer infoDisplayer, ClientGameCard card)
		: base(infoDisplayer)
	{
		Focus(card);
	}

	protected override void Display(ClientGameCard shownCard)
	{
		base.Display(shownCard);

		InfoDisplayer.DisplayZoomed(zoomedIn: false); //For now, assume never zoomed in.

		DisplayTargeting(shownCard);
	}

	public void SetGreyedOut(bool greyedOut)
	{
		this.greyedOut = greyedOut;
		DisplayFrame();
	}

	public void SetBaseFrameColor(Vector4 frameColor)
	{
		this.frameColor = frameColor;
		DisplayFrame();
	}

	private void DisplayFrame()
	{
		InfoDisplayer.DisplayFrame(ShownCard?.OwningPlayer.Friendly ?? false, frameColor);
	}

	protected override void DisplayImage()
	{
		_ = ShownCard ?? throw new System.InvalidOperationException("Can't display image while not showing a card!");
		
		InfoDisplayer.DisplayCardImage(ShownCard);
	}

	private void DisplayTargeting(ClientGameCard shownCard)
	{
		var targetingController = shownCard.ClientGame.ClientGameController.TargetingController
			?? throw new System.NullReferenceException("Forgot to init");
		InfoDisplayer.DisplayValidTarget(targetingController.IsUnselectedValidTarget(shownCard));
		InfoDisplayer.DisplayCurrentTarget(targetingController.IsSelectedTarget(shownCard));
	}
}