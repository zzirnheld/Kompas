using Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.MainMenu;

namespace Kompas.Shared.Controllers
{
	public partial class EscapeMenuController2 : Node
	{
		private const float OpenDuration = 1f;

		[Export]
		private SpinningLogoStateMachine? _spinningLogo;
		private SpinningLogoStateMachine SpinningLogo => _spinningLogo
			?? throw new UnassignedReferenceException(nameof(_spinningLogo), this);

		[Export]
		private Control? _spinningLogoImage;
		private Control SpinningLogoImage => _spinningLogoImage
			?? throw new UnassignedReferenceException(nameof(_spinningLogoImage), this);

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
			Closed = SpinningLogoStateMachine.Positioning.Of(SpinningLogoImage);
			SpinningLogo.RenameCurrentState(SpinningLogoStateMachine.Destination.Closed);
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
				InitialProgress = SpinningLogo.Target.Destination == SpinningLogoStateMachine.Destination.Closed
					? 1 - SpinningLogo.Progress
					: 0f,
			});
		}

		private void Close()
		{
			Logger.Log("Closing!");
			SpinningLogo.LookTowards(new(OpenDuration, SpinningLogoStateMachine.Destination.Closed, Closed)
			{
				InitialProgress = SpinningLogo.Target.Destination == SpinningLogoStateMachine.Destination.Open
					? 1 - SpinningLogo.Progress
					: 0f,
			});
		}
	}
}