using System;
using Godot;
using Kompas.Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.MainMenu;

namespace Kompas.Shared.Controllers
{
	public partial class EscapeMenuController : Node
	{
		private const float FullClockwiseRotation = 2f * System.MathF.PI;
		private const float OpenDuration = 1f;

		[Export]
		private SpinningLogoStateMachine? _spinningLogo;
		private SpinningLogoStateMachine SpinningLogo => _spinningLogo
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

		private readonly SpinningLogoStateMachine.Positioning Opened = new()
		{
			Rotation = (float)(-(6f / 4f) * System.MathF.PI),

			LeftAnchor = -2f,
			RightAnchor = 0.8f,
			TopAnchor = 0f,
			BottomAnchor = 1f,

			LeftOffset = 0f,
			RightOffset = 0f,
			TopOffset = 0f,
			BottomOffset = 0f,
		};

		private SpinningLogoStateMachine.Positioning? _closed;
		private SpinningLogoStateMachine.Positioning Closed
		{
			get => _closed ?? throw new NotReadyYetException();
			set => _closed = value;
		}

		public override void _Ready()
		{
			var startingState = SpinningLogoStateMachine.Positioning.Of(SpinningLogoImage);
			Closed = startingState.With(rotation: startingState.Rotation + FullClockwiseRotation); //So that we always end up circling back around before going
			SpinningLogo.RenameCurrentState(SpinningLogoStateMachine.Destination.Closed);
		}

		public readonly struct ButtonData
		{
			public string Text { get; init; }
			public Action OnClick { get; init; }
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
				button.MouseEntered += () =>
				{
					var targetRotation = SpinningLogo.RotationForVectorIfAt(button.GlobalCenter(), SpinningLogo.Target.Positioning);
					var newTarget = SpinningLogo.Target.Copy(
						newPositioning: SpinningLogo.Target.Positioning.With(
							rotation: targetRotation
						)
					);
					SpinningLogo.LookTowards(newTarget);
				};

				ButtonsParent.AddChild(button);
			}
		}

		public override void _Input(InputEvent inputEvent)
		{
			if (inputEvent is InputEventKey keyEvent && keyEvent.Keycode == Key.Escape && !keyEvent.Pressed) Toggle();
		}

		private void Toggle()
		{
			Logger.Log("Toggling!");
			//We want to open if we're closed, but otherwise toggling the menu closes it, no matter what state we're in.
			//TODO here forbid closing if we're currently spinning out to leave the scene?
			if (SpinningLogo.Target.Destination == SpinningLogoStateMachine.Destination.Closed) Open();
			else Close();
		}

		private void Open()
		{
			Logger.Log("Opening!");
			SpinningLogo.LookTowards(new(OpenDuration, SpinningLogoStateMachine.Destination.Open, Opened)
			{
				NormalizeOnDeparture = true,
				InitialProgress = SpinningLogo.Target.Destination == SpinningLogoStateMachine.Destination.Closed
					? 1 - SpinningLogo.Progress
					: 0f,
				AdditionalStep = progress => UpdateHaze(progress), 
			});
		}

		private void Close()
		{
			Logger.Log("Closing!");
			SpinningLogo.LookTowards(new(OpenDuration, SpinningLogoStateMachine.Destination.Closed, Closed)
			{
				NormalizeOnDeparture = true,
				InitialProgress = SpinningLogo.Target.Destination == SpinningLogoStateMachine.Destination.Open
					? 1 - SpinningLogo.Progress
					: 0f,
				AdditionalStep = progress => UpdateHaze(1 - progress),
			});
		}

		private void UpdateHaze(float progress)
		{
			EscapeMenuButtons.Modulate = new(1f, 1f, 1f, progress);
			EscapeMenuHaze.Modulate = new(1f, 1f, 1f, progress);
		}
	}
}