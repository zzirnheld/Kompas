using System;
using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class SplashScreenRotatingLogo : RotatingTextureRect
	{
		[Export]
		public Control LeftSpacer;

		private bool go = false;

		public override void _Ready()
		{
			RotationDuration = 2f;
			RotateTowards((float)(-0.25f * Math.PI));
		}

		public void SpinOut()
		{
			if (go) return;
			go = true;

			RotateTowards((float)(-2f * Math.PI));
		}

		protected override void Progress(float x)
		{
			base.Progress(x);
			if (go) LeftSpacer.SizeFlagsStretchRatio = x;
		}
	}
}