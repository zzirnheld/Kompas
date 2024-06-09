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
		//Factor out into a shared logo spinning constants file? for consistency across different loading screens
		public const float FullCircleDuration = 2f;
		private const float FirstWipeDuration = 1f / 4f * FullCircleDuration;
		private const float ButtonChooseDuration = 0.5f;
		private const float NewSceneWipeDuration
			= (DestinationRotationWhenSpinningPastMenuForNewScene - StartRotationWhenSpinningPastMenuForNewScene)
			/ FullClockwiseRotation
			* FullCircleDuration;

		private const float FullClockwiseRotation = 2f * MathF.PI;

		private const float DestinationRotationWhenFinishingLoading = -11f / 8f * FullClockwiseRotation;
		private const float WhenMakeTopRightInvisible = DestinationRotationWhenFinishingLoading + (1f / 2f * FullClockwiseRotation);
		private const float PastClickHereToStartRotation = -3f / 8f * FullClockwiseRotation;
		private const float DestinationRotationWhenSpinningPastMenuForNewScene = -1f / 8f * FullClockwiseRotation;
		private const float StartRotationWhenSpinningPastMenuForNewScene = -3f / 4f * FullClockwiseRotation;

		private const float EndSplashLeftAnchor = 0f;
		private const float EndSplashRightAnchor = 2f;
		private const float LeftBufferStretchSize = 0.2f;

		private static bool FirstLoad = true;

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

		public override async void _Ready()
		{
			if (!FirstLoad)
			{
				await SkipToLoadedMenu();
				return;
			}
			
			//Before anything else, start loading the cards
			var loadingThread = Task.Run(MainMenuCardRepository.Load);
			Start = LogoController.CurrentPositioning;

			ClickToStart.Pressed += () => SplashScreenClicked(loadingThread);
		}

		public async void SplashScreenClicked(Task loadTask)
		{
			//Wipe the splash screen off
			var pastClickHereToStart = Start.With(rotation: PastClickHereToStartRotation);
			await LogoController.LookTowards(new(FirstWipeDuration, LogoSpinController.Destination.Destination, pastClickHereToStart)
			{
				RotationProportion = x => x,
			});
			await SpinUntilLoad(loadTask);
		}

		private async Task SpinUntilLoad(Task loadTask)
		{
			TopLeft.Visible = true;
			ClickToStartParent.Visible = false;

			var now = Time.GetTicksMsec();
			// Spin task never completes, but we want to await it to catch any errors.
			await Task.WhenAny(loadTask,
				LogoController.Spin(FullCircleDuration, LogoSpinController.SpinDirection.Clockwise));
			// Once we finish loading, we load the menu.
			await LoadMenu(now);
		}

		private async Task LoadMenu(ulong startMsec)
		{
			var destinationRotation = DestinationRotationWhenFinishingLoading;
			var pastMenu = Start.With(rotation: destinationRotation, leftAnchor: EndSplashLeftAnchor, rightAnchor: EndSplashRightAnchor);
			var duration = MathF.Abs(FullCircleDuration * ((LogoController.ToControl.Rotation - destinationRotation) / FullClockwiseRotation));
			await LogoController.LookTowards(new(duration, LogoSpinController.Destination.Destination, pastMenu)
			{
				RotationProportion = x => x,
				AdditionalStep = progress => {
					//TODO refactor somehow, possibly to make arguments include rotation?
					if (LogoController.ToControl.Rotation < WhenMakeTopRightInvisible) TopRight.Visible = false;
					LeftBufferForNotCoveringUpButtons.SizeFlagsStretchRatio = LogoSpinController.TransitionTarget.Cubic(progress) * LeftBufferStretchSize;
				},
			});

			loaded = true;

			LookTowards(lookAtNext);
			Logger.Log($"Loading took {Time.GetTicksMsec() - startMsec} ms after reaching the point where we'd start spinning");
			FirstLoad = false;
		}

		private async Task SkipToLoadedMenu()
		{
			Start = LogoController.CurrentPositioning;

			var destinationRotation = StartRotationWhenSpinningPastMenuForNewScene;
			var startSkip = Start.With(rotation: destinationRotation, leftAnchor: EndSplashLeftAnchor, rightAnchor: EndSplashRightAnchor);

			//TODO add a method on LogoController that does looktowards.wait() to factor out this shared logic
			LogoController.LookTowards(new(0f, LogoSpinController.Destination.Destination, startSkip))
				.Wait();

			TopLeft.Visible = false;
			ClickToStartParent.Visible = false;
			LeftBufferForNotCoveringUpButtons.SizeFlagsStretchRatio = LeftBufferStretchSize;

			loaded = true;

			TopRight.Visible = true;
			await LogoController.LookTowards(new(NewSceneWipeDuration, LogoSpinController.Destination.Destination,
				startSkip.With(rotation: DestinationRotationWhenSpinningPastMenuForNewScene)));
			TopRight.Visible = false;
		}

		//Event handler for main menu buttons
		public async void LookTowards(Button? button)
		{
			if (!loaded)
			{
				lookAtNext = button;
				return;
			}

			if (button == null) return;

			await LookAtButton(button);
		}

		private async Task LookAtButton(Button button)
		{
			LogoController.NormalizeAngle();
			TopLeft.Visible = false;
			TopRight.Visible = false;
			LeftBufferForNotCoveringUpButtons.SizeFlagsStretchRatio = LeftBufferStretchSize;

			var targetRotation = LogoController.RotationForVectorIfAt(button.GlobalCenter(), LogoController.Target.Positioning);
			var positioning = LogoController.Target.Positioning.With(rotation: targetRotation);
			await LogoController.LookTowards(new(ButtonChooseDuration, LogoSpinController.Destination.Destination, positioning));
		}

		public async Task SpinForSceneChange()
		{
			var destinationRotation = DestinationRotationWhenFinishingLoading;

			var positioning = Start.With(rotation: destinationRotation);
			var duration = MathF.Abs(FullCircleDuration * ((LogoController.ToControl.Rotation - destinationRotation) / FullClockwiseRotation));
			await LogoController.LookTowards(new(duration, LogoSpinController.Destination.Destination, positioning)
			{
				RotationProportion = x => x,
				AdditionalStep = progress => {
					//TODO refactor somehow, possibly to make arguments include rotation?
					if (LogoController.ToControl.Rotation > WhenMakeTopRightInvisible)
					{
						TopLeft.Visible = true;
						TopRight.Visible = true;
					};
					LeftBufferForNotCoveringUpButtons.SizeFlagsStretchRatio = LogoSpinController.TransitionTarget.Cubic(1 - progress) * LeftBufferStretchSize;
				},
			});
			Logger.Log($"Spun to poitning down for scene change!");
		}

		public async Task ChangeScenesLoadingSpinning()
		{
			await LogoController.Spin(FullCircleDuration, LogoSpinController.SpinDirection.CounterClockwise);
		}
	}
}