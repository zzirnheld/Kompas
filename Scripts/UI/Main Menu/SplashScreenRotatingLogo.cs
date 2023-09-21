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
		private bool splashScreenOver = false;

		private float startLeftAnchor;
		private float startRightAnchor;
		private const float EndLeftAnchor = 0f;
		private const float EndRightAnchor = 2f;
		private const float SplashScreenAnimationDuration = 2.5f;
		private const float SplashScreenStartRadians = (float)(-0.25f * Math.PI);
		private const float SplashScreenEndRadians = (float)(-2.75f * Math.PI);

		public override void _Ready()
		{
			startLeftAnchor = AnchorLeft;
			startRightAnchor = AnchorRight;

			RotationDuration = SplashScreenAnimationDuration;
			RotateTowards(SplashScreenStartRadians);
		}

		public void SpinOut()
		{
			go = true;
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
				RotationDuration = 0.5f;
			}
		}

		protected override void Progress(float x)
		{
			//TODO disable top parent's collider only once it has come to rest
			base.Progress(x);
			if (!go || splashScreenOver) return;
			
			LeftSpacer.SizeFlagsStretchRatio = x;
			AnchorLeft = startLeftAnchor + (EndLeftAnchor - startLeftAnchor) * x;
			AnchorRight = startRightAnchor + (EndRightAnchor - startRightAnchor) * x;

			if(!coveredMainMenu && Rotation <= -Math.PI)
			{
				coveredMainMenu = true;
				TopLeft.Visible = true;

				foreach(var ctrl in DisappearDuringTransition) ctrl.Visible = false;
			}
			else if (!passedVertical && Rotation <= -2f * Math.PI)
			{
				passedVertical = true;
				TopRight.Visible = false;
				BottomRight.Visible = false;
			}
		}
	}
}