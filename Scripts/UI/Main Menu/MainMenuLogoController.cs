using System;
using System.Threading;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenuLogoController : Node
	{
		private const float FullClockwiseRotation = 2 * MathF.PI;
		private const float FullCircleDuration = 2f;
		private const float FirstWipeDuration = 1f / 4f * FullCircleDuration;

		[Export]
		private SpinningLogoStateMachine? _logoController;
		private SpinningLogoStateMachine LogoController => _logoController
			?? throw new UnassignedReferenceException(nameof(_logoController), this);


		private SpinningLogoStateMachine.Positioning? _start;
		private SpinningLogoStateMachine.Positioning Start
		{
			get => _start ?? throw new NotReadyYetException();
			set => _start = value;
		}

		public override void _Ready()
		{
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
			Thread thread = new(CardLoader.LoadCards);
			thread.Start();
			var now = Time.GetTicksMsec();
			LogoController.SpinCounterClockwise(FullCircleDuration, progress => IfThreadCompleteLoadMenu(thread, now));
		}

		public class CardLoader
		{
			public static void LoadCards()
			{
				new MainMenuCardRepository().Initialize();
			}
		}

		private void IfThreadCompleteLoadMenu(Thread thread, ulong startMsec)
		{
			//Logger.Warn($"Was the repo initialized? {MainMenuCardRepository.Initialized}");
			if (!thread.IsAlive)
			{
				var pastMenu = Start.With(rotation: -3f / 8f * FullClockwiseRotation);
				//TODO make duration calculated by distance from here + distance needed to spin
				LogoController.LookTowards(new(FirstWipeDuration, SpinningLogoStateMachine.Destination.Destination, pastMenu));
				Logger.Log($"Loading took {Time.GetTicksMsec() - startMsec} ms");
			}
		}
	}
}