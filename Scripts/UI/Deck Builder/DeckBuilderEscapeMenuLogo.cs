using Godot;
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
		private Control EscapeMenuHaze { get; set; }
		[Export]
		private Control EscapeMenuButtons { get; set; }
		[Export]
		private Control EscapeMenuParentToSetVisibility { get; set; }
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
			var target = Time >= RotationDuration ? closed.With(FullClockwiseRotation) : closed;
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
			GD.Print($"Starting escape menu rotation when time is {Time}, rotation duration was {rotationDuration}");
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