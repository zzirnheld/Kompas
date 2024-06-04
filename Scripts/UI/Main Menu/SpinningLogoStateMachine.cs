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
		private float progress;
		private float rotationDuration;

		[Export]
		private Control? _toControl;
		private Control ToControl => _toControl
			?? throw new UnassignedReferenceException(nameof(_toControl), this);

		[Export]
		private Control? _centerOfControlled;
		private Control CenterOfControlled => _centerOfControlled
			?? throw new UnassignedReferenceException(nameof(_centerOfControlled), this);

		private TransitionTarget? _target = null;
		protected TransitionTarget Target
		{
			get => _target ?? throw new NotReadyYetException();
			set => _target = value;
		}

		private Positioning start;

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

			public bool NormalizeAngleOnArrival { get; init; } = true;

			public TransitionTarget(float duration, Destination destination, Positioning positioning)
			{
				Duration = duration;
				Destination = destination;
				Positioning = positioning;
			}
		}

		public override void _Ready()
		{
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
					progress += (float) (delta / rotationDuration);
					if (progress < 1f) Progress();
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
		private void Progress()
		{
			ToControl.Rotation = start.Rotation + ((Target.Positioning.Rotation - start.Rotation) * Target.RotationProportion(progress));

			float anchorProportion = Target.AnchorProportion(progress);
			ToControl.AnchorLeft   = start.LeftAnchor   + (Target.Positioning.LeftAnchor   - start.LeftAnchor)   * anchorProportion;
			ToControl.AnchorRight  = start.RightAnchor  + (Target.Positioning.RightAnchor  - start.RightAnchor)  * anchorProportion;
			ToControl.AnchorTop    = start.TopAnchor    + (Target.Positioning.TopAnchor    - start.TopAnchor)    * anchorProportion;
			ToControl.AnchorBottom = start.BottomAnchor + (Target.Positioning.BottomAnchor - start.BottomAnchor) * anchorProportion;

			float offsetProportion = Target.OffsetProportion(progress);
			ToControl.OffsetLeft   = start.LeftOffset   + (Target.Positioning.LeftOffset   - start.LeftOffset)   * offsetProportion;
			ToControl.OffsetRight  = start.RightOffset  + (Target.Positioning.RightOffset  - start.RightOffset)  * offsetProportion;
			ToControl.OffsetTop    = start.TopOffset    + (Target.Positioning.TopOffset    - start.TopOffset)    * offsetProportion;
			ToControl.OffsetBottom = start.BottomOffset + (Target.Positioning.BottomOffset - start.BottomOffset) * offsetProportion;
		}

		private void Arrive()
		{
			ToControl.Rotation = Target.Positioning.Rotation;

			ToControl.AnchorTop = Target.Positioning.TopAnchor;
			ToControl.AnchorBottom = Target.Positioning.BottomAnchor;
			ToControl.AnchorLeft = Target.Positioning.LeftAnchor;
			ToControl.AnchorRight = Target.Positioning.RightAnchor;

			ToControl.OffsetTop = Target.Positioning.TopOffset;
			ToControl.OffsetBottom = Target.Positioning.BottomOffset;
			ToControl.OffsetLeft = Target.Positioning.LeftOffset;
			ToControl.OffsetRight = Target.Positioning.RightOffset;

			if (Target.NormalizeAngleOnArrival) NormalizeAngle();

			start = Target.Positioning;
			state = State.Stationary;
		}

		private void NormalizeAngle()
		{
			while (ToControl.Rotation > System.MathF.PI) ToControl.Rotation -= FullClockwiseRotation;
			while (ToControl.Rotation < -System.MathF.PI) ToControl.Rotation += FullClockwiseRotation;
		}

		private float RotationForVector(Vector2 targetPosition)
			=> Mathf.Atan2(targetPosition.X 					- CenterOfControlled.GlobalPosition.X,
						   CenterOfControlled.GlobalPosition.Y 	- targetPosition.Y);
	}
}

