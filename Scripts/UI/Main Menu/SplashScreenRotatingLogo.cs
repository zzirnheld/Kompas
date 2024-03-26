using System;
using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class SplashScreenRotatingLogo : RotatingTextureRect
	{
		private const float EndSplashLeftAnchor = 0f;
		private const float EndSplashRightAnchor = 2f;
		private const float SplashScreenAnimationDuration = 2.5f;
		private const float SplashScreenStartRadians = (float)(-0.25f * Math.PI);
		private const float SplashScreenEndRadians = (float)(-2.75f * Math.PI);
		private const float MainMenuRotationDuration = 0.4f;
		private const double UpsideDown = -Math.PI;
		private const double SpunBackAround = -2f * Math.PI;

		[Export]
		private Control? _leftSpacer;
		private Control LeftSpacer => _leftSpacer
			?? throw new UnassignedReferenceException();

		[Export]
		private Control? _topLeft;
		private Control TopLeft => _topLeft
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _topRight;
		private Control TopRight => _topRight
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _bottomLeft;
		private Control BottomLeft => _bottomLeft
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _bottomRight;
		private Control BottomRight => _bottomRight
			?? throw new UnassignedReferenceException();

		[Export]
		private Control[]? _disappearDuringTransition;
		private Control[] DisappearDuringTransition => _disappearDuringTransition
			?? throw new UnassignedReferenceException();

		private bool splashScreenStarted = false;
		private bool splashScreenOver = false;

		private bool coveredMainMenu = false;
		private bool passedVertical = false;

		private float startLeftAnchor;
		private float startRightAnchor;

		protected override float InitialRotation => SplashScreenStartRadians;
		private float rotationDuration;
		protected override float RotationDuration => rotationDuration;

		private bool InSplashScreenRotation => splashScreenStarted && !splashScreenOver;
		protected override bool ArriveBeforeStartingNext => InSplashScreenRotation;

		public override void _Ready()
		{
			startLeftAnchor = AnchorLeft;
			startRightAnchor = AnchorRight;

			rotationDuration = SplashScreenAnimationDuration;
			RotateTowards(SplashScreenStartRadians); //TODO figure out how to not need this call
		}

		public void SpinOut()
		{
			//TODO: the top right and bottom left are blocking corners of the main menu from receiving clicks, so consider adding logic to disable their colliders until spin starts
			Logger.Log("Spin out!");
			RotateTowards(start => start.With(rotation: SplashScreenEndRadians, leftAnchor: EndSplashLeftAnchor, rightAnchor: EndSplashRightAnchor));
			splashScreenStarted = true;
		}

		protected override void Arrive()
		{
			base.Arrive();
			if (InSplashScreenRotation)
			{
				splashScreenOver = true;
				TopLeft.Visible = false;
				BottomLeft.Visible = false;
				LeftSpacer.SizeFlagsStretchRatio = 1f;
				rotationDuration = MainMenuRotationDuration;
			}
		}

		protected override void Progress(float x)
		{
			base.Progress(x);
			if (!splashScreenStarted || splashScreenOver) return;

			LeftSpacer.SizeFlagsStretchRatio = x;
			
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