using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenuLogoController : Node
	{
		private const float FullClockwiseRotation = 2 * MathF.PI;
		private const float FullCircleDuration = 2f;
		private const float FirstWipeDuration = 1f / 4f * FullCircleDuration;
		private const float ButtonChooseDuration = 0.5f;

		private const float DestinationRotationWhenFinishingLoading = -11f / 8f * FullClockwiseRotation;
		private const float WhenMakeTopRightInvisible = DestinationRotationWhenFinishingLoading + (1f / 2f * FullClockwiseRotation);
		private const float EndSplashLeftAnchor = 0f;
		private const float EndSplashRightAnchor = 2f;
		private const float LeftBufferStretchSize = 0.2f;
		private const float PastClickHereToStartRotation = -3f / 8f * FullClockwiseRotation;

		[Export]
		private LogoSpinController? _logoController;
		private LogoSpinController LogoController => _logoController
			?? throw new UnassignedReferenceException(nameof(_logoController), this);

		[Export]
		private Control? _leftBufferForNotCoveringUpButtons;
		private Control LeftBufferForNotCoveringUpButtons => _leftBufferForNotCoveringUpButtons
			?? throw new UnassignedReferenceException(nameof(_leftBufferForNotCoveringUpButtons), this);

		[Export]
		private Control? _topLeft;
		private Control TopLeft => _topLeft
			?? throw new UnassignedReferenceException(nameof(_topLeft), this);

		[Export]
		private Control? _topRight;
		private Control TopRight => _topRight
			?? throw new UnassignedReferenceException(nameof(_topRight), this);

		[Export]
		private Control? _clickToStartParent;
		private Control ClickToStartParent => _clickToStartParent
			?? throw new UnassignedReferenceException(nameof(_clickToStartParent), this);

		[Export]
		private Button? _clickToStart;
		private Button ClickToStart => _clickToStart
			?? throw new UnassignedReferenceException(nameof(_clickToStart), this);

		private LogoSpinController.Positioning? _start;
		private LogoSpinController.Positioning Start
		{
			get => _start ?? throw new NotReadyYetException();
			set => _start = value;
		}

		private bool loaded = false;
		private Button? lookAtNext;

		public override void _Ready()
		{
			//Before anything else, start loading the cards
			var loadingThread = Task.Run(MainMenuCardRepository.Load);
			Start = LogoController.CurrentPositioning;

			ClickToStart.Pressed += () => SplashScreenClicked(loadingThread);
		}

		public void SplashScreenClicked(Task loadTask)
		{
			//Wipe the splash screen off
			var pastClickHereToStart = Start.With(rotation: PastClickHereToStartRotation);
			LogoController.LookTowards(new(FirstWipeDuration, LogoSpinController.Destination.Destination, pastClickHereToStart)
			{
				RotationProportion = x => x,
				OnArrival = () => SpinUntilLoad(loadTask),
			});
		}

		private void SpinUntilLoad(Task loadTask)
		{
			TopLeft.Visible = true;
			ClickToStartParent.Visible = false;

			var now = Time.GetTicksMsec();
			LogoController.SpinCounterClockwise(FullCircleDuration, progress => IfThreadCompleteLoadMenu(loadTask, now));
		}

		private void IfThreadCompleteLoadMenu(Task loadTask, ulong startMsec)
		{
			if (loadTask.IsCompleted)
			{
				var destinationRotation = DestinationRotationWhenFinishingLoading;
				var pastMenu = Start.With(rotation: destinationRotation, leftAnchor: EndSplashLeftAnchor, rightAnchor: EndSplashRightAnchor);
				var duration = MathF.Abs(FullCircleDuration * ((LogoController.ToControl.Rotation - destinationRotation) / FullClockwiseRotation));
				LogoController.LookTowards(new(duration, LogoSpinController.Destination.Destination, pastMenu)
				{
					RotationProportion = x => x,
					AdditionalStep = progress => {
						//TODO refactor somehow, possibly to make arguments include rotation?
						if (LogoController.ToControl.Rotation < WhenMakeTopRightInvisible) TopRight.Visible = false;
						LeftBufferForNotCoveringUpButtons.SizeFlagsStretchRatio = LogoSpinController.TransitionTarget.Cubic(progress) * LeftBufferStretchSize;
					},
					OnArrival = () => {
						loaded = true;
						LookTowards(lookAtNext);
					},
				});

				Logger.Log($"Loading took {Time.GetTicksMsec() - startMsec} ms after reaching the point where we'd start spinning");
			}
		}

		public void LookTowards(Button? button)
		{
			if (!loaded)
			{
				lookAtNext = button;
				return;
			}

			if (button == null) return;

			LogoController.NormalizeAngle();
			TopLeft.Visible = false;
			LeftBufferForNotCoveringUpButtons.SizeFlagsStretchRatio = LeftBufferStretchSize;

			var targetRotation = LogoController.RotationForVectorIfAt(button.GlobalCenter(), LogoController.Target.Positioning);
			var positioning = LogoController.Target.Positioning.With(rotation: targetRotation);
			LogoController.LookTowards(new(ButtonChooseDuration, LogoSpinController.Destination.Destination, positioning));
		}
	}
}