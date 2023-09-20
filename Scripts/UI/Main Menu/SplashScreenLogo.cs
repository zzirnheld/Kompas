using System;
using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class SplashScreenLogo : Control
	{
		[Export]
		public RotatingTextureRect RotatingTextureRect { get; set; }

		private bool go = false;

		public override void _Ready()
		{
			RotatingTextureRect.RotateTowards((float)(-0.25f * Math.PI));
		}

		public void SpinOut()
		{
			if (go) return;
			go = true;

			RotatingTextureRect.RotateTowards((float)(-2f * Math.PI));
		}
	}
}