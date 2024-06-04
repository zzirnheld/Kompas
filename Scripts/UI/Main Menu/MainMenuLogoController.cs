using System;
using System.Threading;
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

		[Export]
		private SpinningLogoStateMachine? _logoController;
		private SpinningLogoStateMachine LogoController => _logoController
			?? throw new UnassignedReferenceException(nameof(_logoController), this);

		[Export]
		private Control? _topLeft;
		private Control TopLeft => _topLeft
			?? throw new UnassignedReferenceException(nameof(_topLeft), this);

		[Export]
		private Control? _topRight;
		private Control TopRight => _topRight
			?? throw new UnassignedReferenceException(nameof(_topRight), this);

		[Export]
		private Control? _clickToStart;
		private Control ClickToStart => _clickToStart
			?? throw new UnassignedReferenceException(nameof(_clickToStart), this);

		private SpinningLogoStateMachine.Positioning? _start;
		private SpinningLogoStateMachine.Positioning Start
		{
			get => _start ?? throw new NotReadyYetException();
			set => _start = value;
		}

		private Thread? loadingThread;

		public override void _Ready()
		{
			loadingThread = new(CardLoader.LoadCards);
			loadingThread.Start();
			Start = SpinningLogoStateMachine.Positioning.Of(LogoController.ToControl); //TODO factor this out to a .CurrentPositioning
			FirstThing();
		}

		public void FirstThing()
		{
			//Wipe the splash screen off
			var pastClickHereToStart = Start.With(rotation: -3f / 8f * FullClockwiseRotation);
			LogoController.LookTowards(new(FirstWipeDuration, SpinningLogoStateMachine.Destination.Destination, pastClickHereToStart)
			{
				OnArrival = SpinUntilLoad,
				RotationProportion = x => x,
			});
		}

		private void SpinUntilLoad()
		{
			TopLeft.Visible = true;
			ClickToStart.Visible = false;

			var now = Time.GetTicksMsec();
			LogoController.SpinCounterClockwise(FullCircleDuration, progress => IfThreadCompleteLoadMenu(now));
		}

		public class CardLoader
		{
			public static void LoadCards()
			{
				new MainMenuCardRepository().Initialize();
			}
		}

		private const float DestinationRotationWhenFinishingLoading = -11f / 8f * FullClockwiseRotation;
		private const float WhenMakeTopRightInvisible = DestinationRotationWhenFinishingLoading + (1f / 2f * FullClockwiseRotation);

		private void IfThreadCompleteLoadMenu(ulong startMsec)
		{
			if (loadingThread == null) throw new NullReferenceException("Must define the thread before this point!");
			if (!loadingThread.IsAlive)
			{
				var destinationRotation = DestinationRotationWhenFinishingLoading;
				var pastMenu = Start.With(rotation: destinationRotation);
				var duration = MathF.Abs(FullCircleDuration * ((LogoController.ToControl.Rotation - destinationRotation) / FullClockwiseRotation));
				LogoController.LookTowards(new(duration, SpinningLogoStateMachine.Destination.Destination, pastMenu)
				{
					RotationProportion = x => x,
					AdditionalStep = _ => {
						//TODO refactor somehow, possibly to make arguments include rotation?
						if (LogoController.ToControl.Rotation < WhenMakeTopRightInvisible) TopRight.Visible = false;
					},
				});

				Logger.Log($"Loading took {Time.GetTicksMsec() - startMsec} ms after reaching the point where we'd start spinning");
			}
		}

		public void LookTowards(Button button)
		{
			LogoController.NormalizeAngle();
			TopLeft.Visible = false;
			var targetRotation = LogoController.RotationForVectorIfAt(button.GlobalCenter(), LogoController.Target.Positioning);
			var positioning = LogoController.Target.Positioning.With(rotation: targetRotation);
			LogoController.LookTowards(new(ButtonChooseDuration, SpinningLogoStateMachine.Destination.Destination, positioning));
		}
	}
}