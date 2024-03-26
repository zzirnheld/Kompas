using Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.MainMenu;
using System;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderEscapeMenuLogo : RotatingTextureRect
	{
		private const float BaseRotationDuration = 1f;

		private readonly Positioning Opened = new()
		{
			Rotation = (float)(-(6f / 4f) * Math.PI),

			LeftAnchor = -2f,
			RightAnchor = 0.8f,
			TopAnchor = 0f,
			BottomAnchor = 1f,

			LeftOffset = 0f,
			RightOffset = 0f,
			TopOffset = 0f,
			BottomOffset = 0f,
		};
		private Positioning closed;

		[Export]
		private Control? _escapeMenuHaze;
		private Control EscapeMenuHaze => _escapeMenuHaze
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _escapeMenuButtons;
		private Control EscapeMenuButtons => _escapeMenuButtons
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _escapeMenuParentToSetVisibility;
		private Control EscapeMenuParentToSetVisibility => _escapeMenuParentToSetVisibility
			?? throw new UnassignedReferenceException();
		private float initialHazeVisibility;
		private float initialButtonVisibility;
		private float TargetVisibility => open ? 1f : 0f;

		private float rotationDuration = BaseRotationDuration;
		protected override float RotationDuration => rotationDuration;

		private bool open = false;

		public override void _Ready()
		{
			base._Ready();
			closed = Positioning.Of(this);
		}

		protected override void Progress(float x)
		{
			base.Progress(x);
			EscapeMenuHaze.Modulate = Visibility(x * x, initialHazeVisibility);
			EscapeMenuButtons.Modulate = Visibility(ButtonTimeProportion(x), initialButtonVisibility);
		}

		private Color Visibility(float x, float initialVisibility)
			=> new(1f, 1f, 1f, initialVisibility + (x * (TargetVisibility - initialVisibility)));

		private static float ButtonTimeProportion(float x) => (float)Math.Cbrt(x);

		protected override float ManipulateAnchorTimeProportion(float x) => x * x;

		public override void _Input(InputEvent inputEvent)
		{
			if (inputEvent is InputEventKey keyEvent && keyEvent.Keycode == Key.Escape && !keyEvent.Pressed) Toggle();
		}

		public void Toggle()
		{
			if (open) Close();
			else Open();
		}

		public void Open()
		{
			RotateTowards(Opened);
			open = true;
		}

		public void Close()
		{
			//If haven't arrived yet, didn't normalize the angle, so no reason to add the full clockwise rotation
			//I'd not normalize the angle on arrival, except that otherwise selecting the escape menu buttons wouldn't work.
			//If rotation is backwards but not about to be normalized, it means we interrupted the main spin out and should go back to normal without the bonus spin
			//TODO make this more robust, possibly with some flag if it successfully arrived?
			var target = Rotation < 0 && Rotation > -Math.PI ? closed : closed.With(FullClockwiseRotation);
			RotateTowards(target);
			open = false;
		}

		public override void RotateTowards(From from)
		{
			rotationDuration = DetermineRotationDuration();
			NormalizeAngle();
			base.RotateTowards(from);
			initialHazeVisibility = EscapeMenuHaze.Modulate.A;
			initialButtonVisibility = EscapeMenuButtons.Modulate.A;
			EscapeMenuParentToSetVisibility.Visible = true;
		}

		private float DetermineRotationDuration()
		{
			Logger.Log($"Starting escape menu rotation when time is {Time}, rotation duration was {rotationDuration}");
			if (Time > rotationDuration) return BaseRotationDuration;
			else if (Time == 0f) return BaseRotationDuration;
			else return Time;
		}

		protected override void Arrive()
		{
			base.Arrive();
			if (!open) EscapeMenuParentToSetVisibility.Visible = false;
		}
	}
}