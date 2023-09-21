using System;
using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class SplashScreenRotatingLogo : RotatingTextureRect
	{
		private const float EndLeftAnchor = 0f;
		private const float EndRightAnchor = 2f;
		private const float SplashScreenAnimationDuration = 2.5f;
		private const float SplashScreenStartRadians = (float)(-0.25f * Math.PI);
		private const float SplashScreenEndRadians = (float)(-2.75f * Math.PI);
		private const float MainMenuRotationDuration = 0.4f;
		private const double UpsideDown = -Math.PI;
		private const double SpunBackAround = -2f * Math.PI;

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

		private bool splashScreenStarted = false;
		private bool splashScreenOver = false;

		private float startLeftAnchor;
		private float startRightAnchor;

		public override void _Ready()
		{
			startLeftAnchor = AnchorLeft;
			startRightAnchor = AnchorRight;

			RotationDuration = SplashScreenAnimationDuration;
			RotateTowards(SplashScreenStartRadians);
		}

		public void SpinOut()
		{
			//TODO: the top right and bottom left are blocking corners of the main menu from receiving clicks, so consider adding logic to disable their colliders until spin starts
			splashScreenStarted = true;
			RotateTowards(SplashScreenEndRadians);
		}

		private bool coveredMainMenu = false;
		private bool passedVertical = false;

		protected override void Arrive()
		{
			if (!splashScreenOver)
			{
				splashScreenOver = true;
				TopLeft.Visible = false;
				BottomLeft.Visible = false;
				Rotation = targetRotation = (float)(targetRotation + (2f * Math.PI));
				AnchorLeft = EndLeftAnchor;
				AnchorRight = EndRightAnchor;
				RotationDuration = MainMenuRotationDuration;
			}
		}

		protected override void Progress(float x)
		{
			base.Progress(x);
			if (!splashScreenStarted || splashScreenOver) return;
			
			LeftSpacer.SizeFlagsStretchRatio = x;
			AnchorLeft = startLeftAnchor + (EndLeftAnchor - startLeftAnchor) * x;
			AnchorRight = startRightAnchor + (EndRightAnchor - startRightAnchor) * x;

			// <= because negative angles
			if(!coveredMainMenu && Rotation <= UpsideDown)
			{
				coveredMainMenu = true;
				TopLeft.Visible = true;

				foreach(var ctrl in DisappearDuringTransition) ctrl.Visible = false;
			}
			else if (!passedVertical && Rotation <= SpunBackAround)
			{
				passedVertical = true;
				TopRight.Visible = false;
				BottomRight.Visible = false;
			}
		}
	}
}