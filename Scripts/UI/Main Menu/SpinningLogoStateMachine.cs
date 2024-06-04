using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class SpinningLogoStateMachine : Node
	{
		private const float FullClockwiseRotation = 2f * System.MathF.PI;
		private const float FreeSpinningRotationPerSecond = 1f * FullClockwiseRotation;

		public enum State { Stationary, FreeSpinning, Transitioning }
		public enum Destination { Open, Closed, Destination, Spin }

		private State state;
		private Destination destination;
		/// <summary>
		/// Bound by 0-1
		/// </summary>
		public float Progress { get; private set; }

		[Export]
		private Control? _toControl;
		private Control ToControl => _toControl
			?? throw new UnassignedReferenceException(nameof(_toControl), this);

		[Export]
		private Control? _centerOfControlled;
		private Control CenterOfControlled => _centerOfControlled
			?? throw new UnassignedReferenceException(nameof(_centerOfControlled), this);

		private TransitionTarget? _target = null;
		public TransitionTarget Target
		{
			get => _target ?? throw new NotReadyYetException();
			private set => _target = value;
		}

		private Positioning? _start;
		private Positioning Start
		{
			get => _start ?? throw new NotReadyYetException();
			set => _start = value;
		}

		public readonly struct Positioning
		{
			public float Rotation { get; init; }

			public float LeftAnchor { get; init; }
			public float RightAnchor { get; init; }
			public float TopAnchor { get; init; }
			public float BottomAnchor { get; init; }

			public float LeftOffset { get; init; }
			public float RightOffset { get; init; }
			public float TopOffset { get; init; }
			public float BottomOffset { get; init; }

			public static Positioning Of(Control obj)
			{
				return new()
				{
					Rotation = obj.Rotation,

					LeftAnchor = obj.AnchorLeft,
					RightAnchor = obj.AnchorRight,
					TopAnchor = obj.AnchorTop,
					BottomAnchor = obj.AnchorBottom,

					LeftOffset = obj.OffsetLeft,
					RightOffset = obj.OffsetRight,
					TopOffset = obj.OffsetTop,
					BottomOffset = obj.OffsetBottom,
				};
			}

			public Positioning With(float? rotation = null,
				float? leftAnchor = null, float? rightAnchor = null, float? topAnchor = null, float? bottomAnchor = null,
				float? leftOffset = null, float? rightOffset = null, float? topOffset = null, float? bottomOffset = null)
			{
				return new()
				{
					Rotation = rotation ?? Rotation,

					LeftAnchor   = leftAnchor 	?? LeftAnchor,
					RightAnchor  = rightAnchor 	?? RightAnchor,
					TopAnchor 	 = topAnchor 	?? TopAnchor,
					BottomAnchor = bottomAnchor ?? BottomAnchor,

					LeftOffset   = leftOffset 	?? LeftOffset,
					RightOffset  = rightOffset 	?? RightOffset,
					TopOffset 	 = topOffset 	?? TopOffset,
					BottomOffset = bottomOffset ?? BottomOffset,
				};
			}

			public override string ToString() => $"Rotation {Rotation},"
				+ $"{LeftAnchor}+{LeftOffset} / {RightAnchor}+{RightOffset} / {TopAnchor}+{TopOffset} / {BottomAnchor}+{BottomOffset}";
		}

		public class TransitionTarget
		{
			public Destination Destination { get; }
			public float Duration { get; }
			public Positioning Positioning { get; }

			public float InitialProgress { get; init; } = 0f;

			/// <summary>
			/// Input and output both bound [0, 1]
			/// </summary>
			public delegate float ProgressToProportion(float progress);
			public static readonly ProgressToProportion Cubic = x => 6 * ((x * x / 2) - (x * x * x / 3));

			public ProgressToProportion AnchorProportion { get; init; } = x => x;
			public ProgressToProportion OffsetProportion { get; init; } = x => x;
			public ProgressToProportion RotationProportion { get; init; } = Cubic;

			public delegate void ProgressStep(float progress);
			public ProgressStep AdditionalStep { get; init; } = _ => { };

			public System.Action OnArrival { get; init; } = () => { };

			/// <summary>
			/// You should normalize on departure if you want to do extra circles before arriving at your target.<br/>
			/// <b>Ex</b>: your destination is closed/open.<br/>
			/// You should ALSO normalize before departure if you were free spinning,
			/// but maybe I should TODO handle that in the free spinning thing?
			/// You should NOT normalize before departure if your desired end state is just to get there,
			/// preserving whatever rotation we started from,<br/>
			/// <b>Ex</b>: your destination is just a button or something (in case we were opening/closing)
			/// </summary>
			public bool NormalizeOnDeparture { get; init; } = false;

			public TransitionTarget(float duration, Destination destination, Positioning positioning)
			{
				Duration = duration;
				Destination = destination;
				Positioning = positioning;
			}

			public TransitionTarget Copy(Positioning? newPositioning = null)
				=> new(Duration, Destination, newPositioning ?? Positioning)
					{
						InitialProgress = InitialProgress,

						AnchorProportion = AnchorProportion,
						OffsetProportion = OffsetProportion,
						RotationProportion = RotationProportion,

						AdditionalStep = AdditionalStep,
						OnArrival = OnArrival,

						NormalizeOnDeparture = NormalizeOnDeparture
					};

			public override string ToString() => $"Duration {Duration}, initial progress {InitialProgress} to Positioning {Positioning}";
		}

		public override void _Ready()
		{
			Start = Positioning.Of(ToControl);
			Target = new(0f, Destination.Destination, Positioning.Of(ToControl));
			ToControl.Resized += () => ToControl.PivotOffset = ToControl.Size / 2;
		}

		public override void _Process(double delta)
		{
			switch (state)
			{
				case State.Stationary: break;
				case State.FreeSpinning:
					ToControl.Rotation += (float) (FreeSpinningRotationPerSecond * delta);
					break;
				case State.Transitioning:
					Progress += (float) (delta / Target.Duration);
					if (Progress < 1f) MakeProgress();
					else Arrive();
					break;
				default:
					throw new System.InvalidOperationException($"Didn't account for  {state}");
			}
		}

		/// <summary>
		/// Progress the rotation towards its endpoint
		/// </summary>
		/// <param name="progress">[0, 1] progress along duration</param>
		private void MakeProgress()
		{
			//Logger.Log($"Progress {Progress}");
			ToControl.Rotation = Start.Rotation + ((Target.Positioning.Rotation - Start.Rotation) * Target.RotationProportion(Progress));

			float anchorProportion = Target.AnchorProportion(Progress);
			ToControl.AnchorLeft   = Start.LeftAnchor   + (Target.Positioning.LeftAnchor   - Start.LeftAnchor)   * anchorProportion;
			ToControl.AnchorRight  = Start.RightAnchor  + (Target.Positioning.RightAnchor  - Start.RightAnchor)  * anchorProportion;
			ToControl.AnchorTop    = Start.TopAnchor    + (Target.Positioning.TopAnchor    - Start.TopAnchor)    * anchorProportion;
			ToControl.AnchorBottom = Start.BottomAnchor + (Target.Positioning.BottomAnchor - Start.BottomAnchor) * anchorProportion;

			float offsetProportion = Target.OffsetProportion(Progress);
			ToControl.OffsetLeft   = Start.LeftOffset   + (Target.Positioning.LeftOffset   - Start.LeftOffset)   * offsetProportion;
			ToControl.OffsetRight  = Start.RightOffset  + (Target.Positioning.RightOffset  - Start.RightOffset)  * offsetProportion;
			ToControl.OffsetTop    = Start.TopOffset    + (Target.Positioning.TopOffset    - Start.TopOffset)    * offsetProportion;
			ToControl.OffsetBottom = Start.BottomOffset + (Target.Positioning.BottomOffset - Start.BottomOffset) * offsetProportion;

			Target.AdditionalStep(Progress);
		}

		private void Arrive()
		{
			Logger.Log($"Arrived at {Target.Positioning}");
			ToControl.Rotation = Target.Positioning.Rotation;

			ToControl.AnchorTop = Target.Positioning.TopAnchor;
			ToControl.AnchorBottom = Target.Positioning.BottomAnchor;
			ToControl.AnchorLeft = Target.Positioning.LeftAnchor;
			ToControl.AnchorRight = Target.Positioning.RightAnchor;

			ToControl.OffsetTop = Target.Positioning.TopOffset;
			ToControl.OffsetBottom = Target.Positioning.BottomOffset;
			ToControl.OffsetLeft = Target.Positioning.LeftOffset;
			ToControl.OffsetRight = Target.Positioning.RightOffset;

			Start = Target.Positioning;
			Progress = 1f;
			state = State.Stationary;
		}

		private void NormalizeAngle()
		{
			ToControl.Rotation = NormalizeAngle(ToControl.Rotation);
		}

		private static float NormalizeAngle(float angle)
		{
			while (angle > System.MathF.PI) angle -= FullClockwiseRotation;
			while (angle < -System.MathF.PI) angle += FullClockwiseRotation;
			return angle;
		}

		private float RotationForVector(Vector2 targetPosition)
			=> Mathf.Atan2(targetPosition.X 					- CenterOfControlled.GlobalPosition.X,
						   CenterOfControlled.GlobalPosition.Y 	- targetPosition.Y);

		public void LookTowards(TransitionTarget target)
		{
			Target = target;
			Progress = Target.InitialProgress;
			state = State.Transitioning;

			if (Target.NormalizeOnDeparture) NormalizeAngle();
			Start = Positioning.Of(ToControl);

			Logger.Log($"Looking from {Start}\ntowards {Target}");
		}

		public void RenameCurrentState(Destination destination)
		{
			state = State.Stationary;
			Target = new(0f, destination, Positioning.Of(ToControl));
			Progress = 1f;
		}
	}
}

