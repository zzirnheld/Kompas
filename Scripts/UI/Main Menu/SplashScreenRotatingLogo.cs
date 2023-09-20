using System;
using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class SplashScreenRotatingLogo : RotatingTextureRect
	{
		[Export]
		private Control LeftSpacer { get; set; }

		[Export]
		private Control TopLeft { get; set; }
		[Export]
		private Control TopRight { get; set; }
		[Export]
		private Control BottomLeft { get; set; }
		[Export]
		private Control BottomRight { get; set; }

		[Export]
		private Control[] DisappearDuringTransition { get; set; }

		private bool go = false;

		public override void _Ready()
		{
			RotationDuration = 2.5f;
			RotateTowards((float)(-0.25f * Math.PI));
		}

		public void SpinOut()
		{
			if (go) return;
			go = true;

			RotateTowards((float)(-3f * Math.PI));
		}

		private bool flag1 = false;
		private bool flag2 = false;
		private bool flag3 = false;

		protected override void Progress(float x)
		{
			base.Progress(x);
			if (!go) return;
			
			LeftSpacer.SizeFlagsStretchRatio = x;

			if(!flag1 && Rotation <= -Math.PI)
			{
				GD.Print("one!");
				flag1 = true;
				TopLeft.Visible = true;

				foreach(var ctrl in DisappearDuringTransition) ctrl.Visible = false;
			}
			else if (!flag2 && Rotation <= -2f * Math.PI)
			{
				GD.Print("two!");
				flag2 = true;
				TopRight.Visible = false;
				BottomRight.Visible = false;
			}
			else if (!flag3 && Rotation <= -3f * Math.PI)
			{
				GD.Print("two!");
				flag2 = true;
				TopLeft.Visible = false;
				BottomLeft.Visible = false;
			}
		}
	}
}