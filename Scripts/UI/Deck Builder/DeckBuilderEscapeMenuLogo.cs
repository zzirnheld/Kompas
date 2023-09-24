using Godot;
using Kompas.UI.MainMenu;
using System;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderEscapeMenuLogo : RotatingTextureRect
	{
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
		private Control EscapeMenuParent { get; set; }
		private float initialEscapeMenuParentModulate;
		private float TargetEscapeMenuParentModulate => open ? 1f : 0f;

		protected override float RotationDuration => 1f;

		private bool open = false;

		public override void _Ready()
		{
			base._Ready();
			closed = Positioning.Of(this);
		}

		protected override void Progress(float x)
		{
			base.Progress(x);
			EscapeMenuParent.Modulate = new Color(1f, 1f, 1f, initialEscapeMenuParentModulate + (x * x * (TargetEscapeMenuParentModulate - initialEscapeMenuParentModulate)));
		}

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
			RotateTowards(closed.With(rotation: FullClockwiseRotation));
			open = false;
		}

		public override void RotateTowards(From from)
		{
			base.RotateTowards(from);
			initialEscapeMenuParentModulate = EscapeMenuParent.Modulate.A;
			EscapeMenuParent.Visible = true;
		}

		protected override void Arrive()
		{
			base.Arrive();
			if (!open) EscapeMenuParent.Visible = false;
		}
	}
}