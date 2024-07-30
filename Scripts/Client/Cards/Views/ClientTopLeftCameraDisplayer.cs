using Godot;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views;

/// <summary>
/// Should be on the node3d that has the camera assigned to the viewport.
/// TODO: explain that this class exists as it does to preserve the focus logic already implemented in FocusableCardViewBase, along with the relevant events.
/// alternately if it becomes necessary to point a camera at a dummy card not actually on the board, we'll cross that bridge.
///</summary>
public partial class ClientTopLeftCameraDisplayer : Control, ICardInfoDisplayer
{
	[Export]
	private TextureRect? _textureRect;
	private TextureRect TextureRect => _textureRect
		?? throw new UnassignedReferenceException();

	[Export]
	private Node3D? _cameraPositionNode;
	private Node3D CameraPositionNode => _cameraPositionNode
		?? throw new UnassignedReferenceException(nameof(_cameraPositionNode), this);

	public bool ShowingInfo { set { } } // => TextureRect.Visible = value; }

	public event System.EventHandler<string>? HoverKeyword;
	public event System.EventHandler<string>? StopHoverKeyword;

	private IHoverableCardInfoDisplayer? lastDisplayed;

	public void Display(CardBase card)
	{
		Logger.Log($"Top left camera Displaying {card}");
		if (lastDisplayed != null)
		{
			lastDisplayed.BeginHoverKeyword -= HoverKeyword;
			lastDisplayed.EndHoverKeyword -= StopHoverKeyword;
		}

		if (card is not GameCard gameCard) throw new System.InvalidOperationException("Can only handle a game card!");
		lastDisplayed = gameCard.CardController.PlaceCameraAboveCard(CameraPositionNode);
		//TODO: make sure we hook up the keywords correctly for mouse hover
		lastDisplayed.BeginHoverKeyword += HoverKeyword;
		lastDisplayed.EndHoverKeyword += StopHoverKeyword;

		HoverKeyword += (_, str) => GD.Print($"Begin {str}");
		StopHoverKeyword += (_, str) => GD.Print($"End {str}");
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