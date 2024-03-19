using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Controllers
{
	public partial class SpaceController : Area3D, ISpaceTargetingController
	{
		private static readonly Vector3 CardOffset = Vector3.Up * 0.002f;

		[Export]
		private SpaceTargetingController? _spaceTargetingController;
		public ISpaceTargetingController SpaceTargetingController => _spaceTargetingController
			?? throw new UnassignedReferenceException();

		public Space Space => throw new NotImplementedException();

		public event EventHandler? LeftClick;

		public override void _Ready()
		{
			base._Ready();
			//CanPlayTo.Visible = false;
			InputEvent += HandleInputEvent;
		}


		private void HandleInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx)
		{
			if (inputEvent is not InputEventMouseButton mouseEvent)
			{
				return;
			}

			//Event where now the mouseEvent is Pressed means it's when the mouse goes down
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				LeftClick?.Invoke(this, EventArgs.Empty);
			}
		}

		public void Place(ICardController card)
		{
			if (card.Node == null)
			{
				GD.PushWarning("Tried to place a card with a null Node!");
				return;
			}

			card.Node.GetParent()?.RemoveChild(card.Node);
			AddChild(card.Node);
			card.Node.Position = -CardOffset;
			var rotation = card.Card.ControllingPlayer.Index * Mathf.Pi;
			card.Node.Rotation = new Vector3(0, rotation, 0);
		}

		public void ToggleHighlight(SpaceHighlight highlight, bool show) => SpaceTargetingController.ToggleHighlight(highlight, show);
	}
}