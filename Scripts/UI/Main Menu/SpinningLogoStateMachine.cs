using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class SpinningLogoStateMachine : Node
	{
		private const float FullClockwiseRotation = 2f * System.MathF.PI;
		private const float FreeSpinningRotationPerSecond = 1f * FullClockwiseRotation;

		public enum State { Stationary, FreeSpinning, Transitioning }
		public enum Destination { Open, Closed, Destination, SpinClockwise, SpinCounterclockwise }

		public State CurrState { get; private set; } = State.Stationary;
		/// <summary>
		/// Bound by 0-1
		/// </summary>
		public float Progress { get; private set; }

		[Export]
		private Control? _toControl;
		public Control ToControl => _toControl
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
			/// <summary>
			/// In the case of a Spin, this is the duration of a full rotation
			/// </summary>
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
			switch (CurrState)
			{
				case State.Stationary: break;
				case State.FreeSpinning:
					ToControl.Rotation += ((Target.Destination == Destination.SpinClockwise) ? 1f : -1f)
						* (float) (FullClockwiseRotation * delta / Target.Duration);
					Target.AdditionalStep(0f);
					break;
				case State.Transitioning:
					Progress += (float) (delta / Target.Duration);
					if (Progress < 1f) MakeProgress();
					else Arrive();
					break;
				default:
					throw new System.InvalidOperationException($"Didn't account for  {CurrState}");
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
			SetPosition(Target.Positioning);

			Start = Target.Positioning;
			Progress = 1f;
			CurrState = State.Stationary;

			Target.OnArrival();
		}

		private void SetPosition(Positioning positioning)
		{
			ToControl.Rotation = positioning.Rotation;

			ToControl.AnchorTop = positioning.TopAnchor;
			ToControl.AnchorBottom = positioning.BottomAnchor;
			ToControl.AnchorLeft = positioning.LeftAnchor;
			ToControl.AnchorRight = positioning.RightAnchor;

			ToControl.OffsetTop = positioning.TopOffset;
			ToControl.OffsetBottom = positioning.BottomOffset;
			ToControl.OffsetLeft = positioning.LeftOffset;
			ToControl.OffsetRight = positioning.RightOffset;
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

		//This only works to get you the rotation given the current center.
		public float RotationForVector(Vector2 targetGlobalPosition)
			=> Mathf.Atan2(targetGlobalPosition.X 				- CenterOfControlled.GlobalPosition.X,
						   CenterOfControlled.GlobalPosition.Y 	- targetGlobalPosition.Y);

		public float RotationForVectorIfAt(Vector2 targetGlobalPosition, Positioning destination)
		{
			var currentPos = Positioning.Of(ToControl);
			SetPosition(destination);
			var ret = RotationForVector(targetGlobalPosition);
			SetPosition(currentPos);
			return ret;
		}

		public void LookTowards(TransitionTarget target)
		{
			Target = target;
			Progress = Target.InitialProgress;
			CurrState = State.Transitioning;

			Start = Positioning.Of(ToControl);

			Logger.Log($"Looking from {Start}\ntowards {Target}");
		}

		public void SpinCounterClockwise(float fullCircleDuration, TransitionTarget.ProgressStep? step = null)
		{
			Target = new(fullCircleDuration, Destination.SpinCounterclockwise, new())
			{
				//If none is provided, default to a no-op
				AdditionalStep = step ?? (_ => { }),
			};
			CurrState = State.FreeSpinning;
		}

		public void RenameCurrentState(Destination destination)
		{
			CurrState = State.Stationary;
			Target = new(0f, destination, Positioning.Of(ToControl));
			Progress = 1f;
		}

		public void SkipTo(TransitionTarget target)
		{
			LookTowards(target);
			Arrive();
		}
	}
}

