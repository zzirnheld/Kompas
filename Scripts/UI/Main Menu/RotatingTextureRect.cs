using System;
using System.Text;
using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class RotatingTextureRect : TextureRect
	{
		protected const float FullClockwiseRotation = (float)(2f * Math.PI);

		[Export]
		private Control? _center;
		public Control Center => _center
			?? throw new UnassignedReferenceException();

		protected virtual float InitialRotation => 0f;

		protected virtual float RotationDuration => 0.5f;

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
		private Positioning? _target = null;
		protected Positioning Target
		{
			get => _target ?? throw new NotReadyYetException();
			set => _target = value;
		}

		private Positioning start;
		protected float Time { get; private set; } = 0f;

		protected virtual bool ArriveBeforeStartingNext => false;
		protected virtual bool NormalizeAngleOnArrival => true;
		//private float currentRotationalVelocity;

		public override void _Ready()
		{
			Rotation = InitialRotation;
			start = Target = Positioning.Of(this).With(rotation: InitialRotation);
			Time = RotationDuration + 1f;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (Time > RotationDuration) return;

			//The cubic that passes through (0,0) and (1,1) that is a dilation of the integral of the integral (-x - 1) is 6(x^2 / 2 - x^3 / 3)
			Time += (float)delta;
			if (Time >= RotationDuration)
			{
				Arrive();
				return;
			}

			Progress(Time / RotationDuration);
		}

		/// <summary>
		/// Progress the rotation towards its endpoint
		/// </summary>
		/// <param name="x">[0, 1] progress along duration</param>
		protected virtual void Progress(float x)
		{
			Rotation = start.Rotation + ((Target.Rotation - start.Rotation) * 6 * ((x * x / 2) - (x * x * x / 3)));

			float anchorX = ManipulateAnchorTimeProportion(x);
			AnchorLeft 	 = start.LeftAnchor   + (Target.LeftAnchor   - start.LeftAnchor)   * anchorX;
			AnchorRight  = start.RightAnchor  + (Target.RightAnchor  - start.RightAnchor)  * anchorX;
			AnchorTop 	 = start.TopAnchor	  + (Target.TopAnchor 	 - start.TopAnchor)	   * anchorX;
			AnchorBottom = start.BottomAnchor + (Target.BottomAnchor - start.BottomAnchor) * anchorX;

			float offsetX = ManipulateOffsetTimeProportion(x);
			OffsetLeft 	 = start.LeftOffset   + (Target.LeftOffset   - start.LeftOffset)   * offsetX;
			OffsetRight  = start.RightOffset  + (Target.RightOffset  - start.RightOffset)  * offsetX;
			OffsetTop 	 = start.TopOffset	  + (Target.TopOffset 	 - start.TopOffset)	   * offsetX;
			OffsetBottom = start.BottomOffset + (Target.BottomOffset - start.BottomOffset) * offsetX;
		}

		protected virtual float ManipulateAnchorTimeProportion(float x) => x;

		protected virtual float ManipulateOffsetTimeProportion(float x) => x;

		protected virtual void Arrive()
		{
			Logger.Log($"Arrived at {Target.Rotation}!");
			Rotation = Target.Rotation;

			AnchorTop = Target.TopAnchor;
			AnchorBottom = Target.BottomAnchor;
			AnchorLeft = Target.LeftAnchor;
			AnchorRight = Target.RightAnchor;

			OffsetTop = Target.TopOffset;
			OffsetBottom = Target.BottomOffset;
			OffsetLeft = Target.LeftOffset;
			OffsetRight = Target.RightOffset;

			if (NormalizeAngleOnArrival) NormalizeAngle();
		}

		protected void NormalizeAngle()
		{
			while (Rotation > Math.PI) Rotation -= FullClockwiseRotation;
			while (Rotation < -Math.PI) Rotation += FullClockwiseRotation;
		}

		//Hook up to TextureRect Resize() function
		public void Resize()
		{
			//Logger.Log($"Resizing. Size is {Size} and rotation is {Rotation}");
			PivotOffset = Size / 2;
		}

		public void LookTowards(Vector2 targetPosition)
		{
			//Logger.Log($"{Name} looking towards {targetPosition}");
			RotateTowards(RotationForVector(targetPosition));
		}

		public void RotateTowards(float angle) => RotateTowards(start => (_target ?? start).With(rotation: angle));

		public void RotateTowards(Positioning target) => RotateTowards(start => target);

		public delegate Positioning From(Positioning start);

		public virtual void RotateTowards(From from)
		{
			if (ArriveBeforeStartingNext)
			{
				Logger.Log($"Arriving at {Target} before starting next rotation");
				Arrive();
			}

			start = Positioning.Of(this);
			Target = from(start);
			Logger.Log($"Rotating from {start} to {Target}");
			Time = 0f;
		}

		private float RotationForVector(Vector2 targetPosition)
			=> Mathf.Atan2(targetPosition.X - Center.GlobalPosition.X,
						   Center.GlobalPosition.Y - targetPosition.Y);
	}
}