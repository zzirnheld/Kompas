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
		private Control? _toMainMenuHaze;
		/// <summary>
		/// Should be visible iff we're going to/from the main menu
		/// </summary>
		private Control ToMainMenuHaze => _toMainMenuHaze
			?? throw new UnassignedReferenceException(nameof(_toMainMenuHaze), this);
		[Export]
		private Control? _escapeMenuButtons;
		private Control EscapeMenuButtons => _escapeMenuButtons
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _escapeMenuParentToSetVisibility;
		private Control EscapeMenuParentToSetVisibility => _escapeMenuParentToSetVisibility
			?? throw new UnassignedReferenceException();

		[Export]
		private Control? _visibleUnlessFullyClosed;
		private Control VisibleUnlessFullyClosed => _visibleUnlessFullyClosed
			?? throw new UnassignedReferenceException();

		[Export]
		private Button? _clickToOpenMenuButton;
		private Button ClickToOpenMenuButton => _clickToOpenMenuButton
			?? throw new UnassignedReferenceException(nameof(_clickToOpenMenuButton), this);

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

		//Making this match the main menu
		//TODO make it a constant that gets skipped to? makes it awkward to mess with in the inspector tho
		private readonly LogoSpinController.Positioning SpinPositioning = new()
		{
			Rotation = (float)(1f / 2f * System.MathF.PI),

			LeftAnchor = 0f,
			RightAnchor = 2f,
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
			//Initialize Closed in _Ready to ensure that closed is only init once node is fully ready
			Closed = startingState.With(rotation: startingState.Rotation + FullClockwiseRotation); //So that we always end up circling back around before going

			ModulateNonMainMenuShowables(0f, showButtons: true);
			FullyClosed();
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

		public async Task SpinForTransitionWithMainMenu(LogoSpinController.SpinDirection spinDirection)
		{
			_ = Closed; //Confirm we have a non-null closed state computed at _Ready first

			ToMainMenuHaze.Visible = true;
			ModulateNormalHaze(1f);
			ModulateButtons(0f, show: false);
			ModulateMainMenuHaze(1f);
			PartiallyOpen();

			await SpinningLogo.LookTowards(new(0f, LogoSpinController.Destination.Spin, SpinPositioning));
			await SpinningLogo.Spin(fullCircleDuration: MainMenuLogoController.FullCircleDuration, spinDirection);
		}

		public async Task PrepareForGoingToMainMenu(float expansionDelay)
		{
			_ = Closed; //Confirm we have a non-null closed state computed at _Ready first
			ToMainMenuHaze.Visible = true;
			ModulateMainMenuHaze(0f);

			await SpinningLogo.LookTowards(new(expansionDelay, LogoSpinController.Destination.Spin, SpinPositioning)
			{
				AdditionalStep = progress =>
				{
					ModulateMainMenuHaze(progress);
					PartiallyOpen();
				},
			});
		}

		public async Task CameFromMainMenuClose()
		{
			await SpinningLogo.LookTowards(new(OpenDuration, LogoSpinController.Destination.Closed, Closed)
			{
				AdditionalStep = progress =>
				{
					ModulateNonMainMenuShowables(1 - progress, showButtons: false);
					ModulateMainMenuHaze(1f - progress);
					PartiallyOpen();
				},

				RotationProportion = x => x,
			});
			FullyClosed();
			ToMainMenuHaze.Visible = false;
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

		//TODO: figure out what allows the toggle to end up rotating it very smol by accident, and fix it
		//Event handler for input event
		private async void Toggle()
		{
			Logger.Log("Toggling!");
			switch (SpinningLogo.Target.Destination)
			{
				case LogoSpinController.Destination.Open:
				case LogoSpinController.Destination.Destination:
					await Close();
					break;
				case LogoSpinController.Destination.Closed:
					await Open();
					break;
				case LogoSpinController.Destination.Spin: //TODO add a better way of making sure we can't toggle while waiting for loading?
					break;
				default: throw new System.InvalidOperationException($"Invalid destination {SpinningLogo.Target.Destination}");
			}
		}

		public async Task Open(bool showButtons = true)
		{
			Logger.Log("Opening!");
			await SpinningLogo.LookTowards(new(OpenDuration, LogoSpinController.Destination.Open, Opened)
			{
				InitialProgress = SpinningLogo.Target.Destination == LogoSpinController.Destination.Closed && SpinningLogo.Moving
					? 1 - SpinningLogo.Progress
					: 0f,
				AdditionalStep = progress =>
				{
					ModulateNonMainMenuShowables(progress, showButtons: showButtons);
					PartiallyOpen();
				},

				RotationProportion = x => x,
			});
			SetButtonsInteractable(true);
		}

		public async Task Close(bool showButtons = true)
		{
			Logger.Log("Closing!");
			SetButtonsInteractable(false);
			await SpinningLogo.LookTowards(new(OpenDuration, LogoSpinController.Destination.Closed, Closed)
			{
				InitialProgress = SpinningLogo.Target.Destination == LogoSpinController.Destination.Open && SpinningLogo.Moving
					? 1 - SpinningLogo.Progress
					: 0f,
				AdditionalStep = progress =>
				{
					ModulateNonMainMenuShowables(1 - progress, showButtons: showButtons);
					PartiallyOpen();
				},

				RotationProportion = x => x,
			});
			FullyClosed();
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

		private void ModulateNonMainMenuShowables(float progress, bool showButtons)
		{
			ModulateButtons(progress, show: showButtons);
			ModulateNormalHaze(System.MathF.Cbrt(progress));

			ClickToOpenMenuButton.AnchorLeft = 0f + (0.25f * progress);
			ClickToOpenMenuButton.AnchorRight = 1f - (0.25f * progress);
			ClickToOpenMenuButton.AnchorTop = 0f + (0.25f * progress);
			ClickToOpenMenuButton.AnchorBottom = 1f - (0.25f * progress);
		}

		private void ModulateButtons(float progress, bool show)
		{
			float alpha = show
				? progress * progress * progress * progress * progress
				: 0f;
			EscapeMenuButtons.Modulate = new(1f, 1f, 1f, alpha);
		}

		private void ModulateNormalHaze(float progress)
		{
			EscapeMenuHaze.Modulate = new(0f, 0f, 0f, progress);
		}

		private void ModulateMainMenuHaze(float progress)
		{
			ToMainMenuHaze.SelfModulate = new(1f, 1f, 1f, System.MathF.Cbrt(progress));
		}

		private void FullyClosed() => VisibleUnlessFullyClosed.Visible = false;
		private void PartiallyOpen() => VisibleUnlessFullyClosed.Visible = true;
	}
}