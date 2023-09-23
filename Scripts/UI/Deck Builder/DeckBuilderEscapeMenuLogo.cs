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

		protected override float RotationDuration => 1f;

		private Positioning closed;

		private bool open = false;

		public override void _Ready()
		{
			base._Ready();
			closed = Positioning.Of(this);
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
	}
}