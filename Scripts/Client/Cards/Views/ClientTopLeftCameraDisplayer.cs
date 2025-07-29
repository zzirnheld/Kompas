using Godot;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.Godot;
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
	private CameraFollowObject? _camera;
	private CameraFollowObject RingCamera => _camera
		?? throw new UnassignedReferenceException(nameof(_camera), this);

	[Export]
	private PackedScene? _reminderTextPrefab;
	private PackedScene ReminderTextPrefab => _reminderTextPrefab
		?? throw new UnassignedReferenceException(nameof(_reminderTextPrefab), this);

	[Export]
	private Control? _reminderTextParent;
	private Control ReminderTextParent => _reminderTextParent
		?? throw new UnassignedReferenceException(nameof(_reminderTextParent), this);

	public bool ShowingInfo { set { } } // => TextureRect.Visible = value; }

	private IHoverableCardInfoDisplayer? lastDisplayed;
	private const uint FocusedCullMask = 1 << (3 - 1) | 1 << (5 - 1);
	private const uint UnfocusedCullMask = 1 << (2 - 1) | 1 << (5 - 1);

	private const uint FocusedLayerMask = 1 << (3 - 1) | 1 << (2 - 1);
	private const uint UnfocusedLayerMask = 1 << (2 - 1);

	public void Display(CardBase card)
	{
		if (card is not GameCard gameCard) throw new System.InvalidOperationException("Can only handle a game card!");

		lastDisplayed?.UpdateZoomedInLayerMask(UnfocusedLayerMask);
		lastDisplayed = gameCard.CardController.PlaceCameraAboveCard(RingCamera, UnfocusedCullMask, FocusedCullMask,
			(infoDisplayer) => infoDisplayer.UpdateZoomedInLayerMask(FocusedLayerMask));

		ReminderTextParent.QueueFreeChildren();
		foreach (var reminderText in gameCard.ReminderTexts)
		{
			var reminderPopup = ReminderTextPrefab.Instantiate() as ReminderTextPopup
				?? throw new System.InvalidOperationException("Wrong type for prefab!");
			reminderPopup.Display(reminderText);
			ReminderTextParent.AddChild(reminderPopup);
		}
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