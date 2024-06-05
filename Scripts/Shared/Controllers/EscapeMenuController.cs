using System.Threading.Tasks;
using Godot;
using Kompas.Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.MainMenu;

namespace Kompas.Shared.Controllers
{
	public partial class EscapeMenuController : Node
	{
		private const float FullClockwiseRotation = 2f * System.MathF.PI;
		private const float OpenDuration = 0.65f;
		private const float SwapDuration = 0.3f;

		[Export]
		private LogoSpinController? _spinningLogo;
		private LogoSpinController SpinningLogo => _spinningLogo
			?? throw new UnassignedReferenceException(nameof(_spinningLogo), this);

		[Export]
		private Control? _spinningLogoImage;
		private Control SpinningLogoImage => _spinningLogoImage
			?? throw new UnassignedReferenceException(nameof(_spinningLogoImage), this);

		[Export]
		private PackedScene? _menuButton;
		private PackedScene MenuButton => _menuButton ?? throw new UnassignedReferenceException(nameof(_menuButton));

		[Export]
		private Control? _buttonsParent;
		private Control ButtonsParent => _buttonsParent ?? throw new UnassignedReferenceException(nameof(_buttonsParent));

		[Export]
		private Control? _escapeMenuHaze;
		private Control EscapeMenuHaze => _escapeMenuHaze
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _escapeMenuButtons;
		private Control EscapeMenuButtons => _escapeMenuButtons
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _escapeMenuParentToSetVisibility;
		private Control EscapeMenuParentToSetVisibility => _escapeMenuParentToSetVisibility
			?? throw new UnassignedReferenceException();

		private readonly LogoSpinController.Positioning Opened = new()
		{
			Rotation = (float)(1f / 2f * System.MathF.PI),

			LeftAnchor = -2f,
			RightAnchor = 0.8f,
			TopAnchor = 0f,
			BottomAnchor = 1f,

			LeftOffset = 0f,
			RightOffset = 0f,
			TopOffset = 0f,
			BottomOffset = 0f,
		};

		private LogoSpinController.Positioning? _closed;
		private LogoSpinController.Positioning Closed
		{
			get => _closed ?? throw new NotReadyYetException();
			set => _closed = value;
		}

		public override void _Ready()
		{
			var startingState = LogoSpinController.Positioning.Of(SpinningLogoImage);
			Closed = startingState.With(rotation: startingState.Rotation + FullClockwiseRotation); //So that we always end up circling back around before going
			SpinningLogo.LookTowards(new(0f, LogoSpinController.Destination.Closed, Closed))
				//This task should complete synchronously, because it has a duration of 0f.
				.Wait();
		}

		public readonly struct ButtonData
		{
			public string Text { get; init; }
			public System.Action OnClick { get; init; }
		}

		private bool initialized = false;

		public void Init(params ButtonData[] buttonsData)
		{
			if (initialized) throw new AlreadyInitializedException();
			initialized = true;

			foreach (var buttonData in buttonsData)
			{
				var button = MenuButton.Instantiate<Button>();

				button.Text = buttonData.Text;
				button.Pressed += buttonData.OnClick;
				//Make button default to being ignored, so as to not redirect the menu until it's fully enabled
				button.MouseFilter = Control.MouseFilterEnum.Ignore;
				button.MouseEntered += () => LookTowards(button);

				ButtonsParent.AddChild(button);
			}
		}

		// Async void because it's an event handler.
		private async void LookTowards(Button button)
		{
			var targetRotation = SpinningLogo.RotationForVectorIfAt(button.GlobalCenter(), SpinningLogo.Target.Positioning);
			var positioning = SpinningLogo.Target.Positioning.With(rotation: targetRotation);

			var duration = SpinningLogo.Target.Destination switch
			{
				LogoSpinController.Destination.Open => SpinningLogo.Moving
					? OpenDuration * (1 - SpinningLogo.Progress)
					: SwapDuration,
				LogoSpinController.Destination.Closed => OpenDuration * SpinningLogo.Progress,
				_ => SwapDuration,
			};
			await SpinningLogo.LookTowards(new(duration, LogoSpinController.Destination.Destination, positioning)
			{
				AnchorProportion = x => x * x,
				OffsetProportion = x => x * x,
				RotationProportion = x => x * x,
			});
		}

		public override void _Input(InputEvent inputEvent)
		{
			if (inputEvent is InputEventKey keyEvent && keyEvent.Keycode == Key.Escape && !keyEvent.Pressed) Toggle();
		}

		//Event handler for input event
		private async void Toggle()
		{
			Logger.Log("Toggling!");
			//We want to open if we're closed, but otherwise toggling the menu closes it, no matter what state we're in.
			//TODO here forbid closing if we're currently spinning out to leave the scene?
			if (SpinningLogo.Target.Destination == LogoSpinController.Destination.Closed) await Open();
			else await Close();
		}

		private async Task Open()
		{
			Logger.Log("Opening!");
			await SpinningLogo.LookTowards(new(OpenDuration, LogoSpinController.Destination.Open, Opened)
			{
				InitialProgress = SpinningLogo.Target.Destination == LogoSpinController.Destination.Closed
					? 1 - SpinningLogo.Progress
					: 0f,
				AdditionalStep = progress => ModulateShowables(progress),
				OnArrival = () => SetButtonsInteractable(true),
			});
		}

		private async Task Close()
		{
			Logger.Log("Closing!");
			SetButtonsInteractable(false);
			await SpinningLogo.LookTowards(new(OpenDuration, LogoSpinController.Destination.Closed, Closed)
			{
				InitialProgress = SpinningLogo.Target.Destination == LogoSpinController.Destination.Open
					? 1 - SpinningLogo.Progress
					: 0f,
				AdditionalStep = progress => ModulateShowables(1 - progress)
			});
		}

		private void SetButtonsInteractable(bool interactable)
		{
			foreach (var button in ButtonsParent.GetChildren())
			{
				if (button is not Control control) continue;
				control.MouseFilter = interactable
					? Control.MouseFilterEnum.Stop
					: Control.MouseFilterEnum.Ignore; 
			}
		}

		private void ModulateShowables(float progress)
		{
			ModulateButtons(progress * progress * progress * progress * progress);
			ModulateHaze(System.MathF.Cbrt(progress));
		}

		private void ModulateButtons(float progress)
		{
			EscapeMenuButtons.Modulate = new(1f, 1f, 1f, progress);
		}

		private void ModulateHaze(float progress)
		{
			EscapeMenuHaze.Modulate = new(0f, 0f, 0f, progress);
		}
	}
}