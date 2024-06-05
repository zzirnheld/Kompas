using System;
using System.Threading.Tasks;
using Godot;
using Kompas.Godot;
using Kompas.Shared;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class LogoSpinController : Node
	{
		private const float FullClockwiseRotation = 2f * System.MathF.PI;

		public enum Destination { Open, Closed, Destination, SpinClockwise, SpinCounterclockwise }

		private class State
		{
			public Positioning Start { get; init; }
			public TransitionTarget Target { get; init; }

			public bool Moving { get; set; } = false;
			public float Progress { get; set; } = 0f;

			public State(Positioning start, TransitionTarget target)
			{
				Start = start;
				Target = target;
			}
		}

		/// <summary>
		/// Recall that lock() is reentrant, so at worst we should never have people colliding there.
		/// Probably still wanna lock around the state lock for any processing you do assuming the current state.
		/// <b>Man</b> it's been a while since I've properly worked around concurrency.
		/// </summary>
		private readonly object stateLock = new();
		private State? _currState;
		/// <summary>
		/// Accesses _currState, locking with the reentrant stateLock()
		/// </summary>
		private State CurrState
		{
			get
			{
				lock (stateLock) { return _currState ?? throw new NotReadyYetException(); }
			}
			set
			{
				lock (stateLock) { _currState = value; }
			}
		}
		public bool Moving => CurrState.Moving;
		/// <summary>
		/// Bound by 0-1
		/// </summary>
		public float Progress
		{
			get => CurrState.Progress;
			private set => CurrState.Progress = value;
		}
		public Positioning Start => CurrState.Start;
		public TransitionTarget Target => CurrState.Target;

		[Export]
		private Control? _toControl;
		public Control ToControl => _toControl
			?? throw new UnassignedReferenceException(nameof(_toControl), this);

		[Export]
		private Control? _centerOfControlled;
		private Control CenterOfControlled => _centerOfControlled
			?? throw new UnassignedReferenceException(nameof(_centerOfControlled), this);

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

		public Positioning CurrentPositioning => Positioning.Of(ToControl);

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
			CurrState = new(CurrentPositioning, new(0f, Destination.Destination, CurrentPositioning));

			ToControl.Resized += () => ToControl.PivotOffset = ToControl.Size / 2;
		}

		/// <summary>
		/// Attempt to look towards a particular target.
		/// </summary>
		/// <returns>True if we successfully got there without changing targets, false otherwise</returns>
		public async Task<bool> LookTowards(TransitionTarget target)
		{
			State state = new(CurrentPositioning, target)
			{
				Moving = true,
				Progress = target.InitialProgress,
			};
			CurrState = state;

			if (target.Duration == 0f)
			{
				Arrive();
				return true;
			}

			Logger.Log($"Looking from {CurrState.Start}\ntowards {target}");

			return await this.DoEachFrame(delta => ProgressLookTowards(target, delta, state));
		}

		private Result<bool> ProgressLookTowards(TransitionTarget target, float delta, State state)
		{
			lock (stateLock)
			{
				//Do this inside the lock, but the lock must not include the await (compiler forbidden, would produce deadlocks)
				if (CurrState != state)
				{
					Logger.Log($"States no longer matched, aborting looking towards {target}");
					return Result<bool>.Of(false);
				}

				Progress += (float)(delta / Target.Duration);

				if (Progress < 1f) MakeProgress();
				else
				{
					Arrive();
					return Result<bool>.Of(true);
				}
			}
			return Result<bool>.None;
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

			CurrState.Moving = false;

			Target.OnArrival();
		}

		public async Task SpinCounterClockwise(float fullCircleDuration)
		{
			State state = new(CurrentPositioning, new(fullCircleDuration, Destination.SpinCounterclockwise, new()))
			{
				Moving = true
			};
			CurrState = state;

			await this.DoEachFrame(delta =>
			{
				lock (stateLock)
				{
					if (CurrState != state) return;

					ToControl.Rotation += ((Target.Destination == Destination.SpinClockwise) ? 1f : -1f)
						* (float)(FullClockwiseRotation * delta / Target.Duration);
					Target.AdditionalStep(0f);
				}
			});
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

		public void NormalizeAngle()
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
	}
}

