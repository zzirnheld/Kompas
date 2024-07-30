using Godot;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views;

/// <summary>
/// Should be on the node3d that has the camera assigned to the viewport.
/// TODO: explain that this class exists as it does to preserve the focus logic already implemented in FocusableCardViewBase, along with the relevant events.
///</summary>
public partial class ClientTopLeftCameraDisplayer : Node, ICardInfoDisplayer
{
	[Export]
	private TextureRect? _viewportTextureRect;
	private TextureRect ViewportTextureRect => _viewportTextureRect
		?? throw new UnassignedReferenceException(nameof(_viewportTextureRect), this);

	public bool ShowingInfo { set => ViewportTextureRect.Visible = value; }

	public event System.EventHandler<string>? HoverKeyword;
	public event System.EventHandler<string>? StopHoverKeyword;

	private ControlInfoDisplayer? lastDisplayed;

	public void Display(CardBase card)
	{
		if (lastDisplayed != null)
		{
			lastDisplayed.HoverKeyword -= HoverKeyword;
			lastDisplayed.StopHoverKeyword -= StopHoverKeyword;
		}

		if (card is not GameCard gameCard) throw new System.InvalidOperationException("Can only handle a game card!");
		lastDisplayed = gameCard.CardController.PlaceCameraAboveCard(this);
		//TODO: make sure we hook up the keywords correctly for mouse hover
		lastDisplayed.HoverKeyword += HoverKeyword;
		lastDisplayed.StopHoverKeyword += StopHoverKeyword;
	}

	//Need this to fit the interface to preserve using the original focus code, but it shouldn't ever call these.
	//TODO refactor the focus logic to not necessarily require this - split focus logic out from the view class
	public void DisplayCardRulesText(CardBase card) => throw new System.NotImplementedException();
	public void DisplayCardNumericStats(CardBase card) => throw new System.NotImplementedException();
	public void DisplayCardImage(CardBase card) => throw new System.NotImplementedException();

	public void DisplayUnselectedValidTarget(bool validTarget) => throw new System.NotImplementedException();
	public void DisplayCurrentTarget(bool currentTarget) => throw new System.NotImplementedException();
	public void DisplayEffectSource(bool effectSource) => throw new System.NotImplementedException();
}