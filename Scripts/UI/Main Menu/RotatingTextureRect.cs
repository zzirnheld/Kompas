using System;
using System.Text;
using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class RotatingTextureRect : TextureRect
	{
		protected const float FullClockwiseRotation = (float)(2f * Math.PI);

		[Export]
		public Control center;

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
		protected Positioning target = new();

		private Positioning start;
		private float time;

		protected virtual bool ArriveBeforeStartingNext => false;
		protected virtual bool NormalizeAngleOnArrival => true;
		//private float currentRotationalVelocity;

		public override void _Ready()
		{
			Rotation = InitialRotation;
			start = target = Positioning.Of(this).With(rotation: InitialRotation);
			time = RotationDuration + 1f;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (time > RotationDuration) return;

			//The cubic that passes through (0,0) and (1,1) that is a dilation of the integral of the integral (-x - 1) is 6(x^2 / 2 - x^3 / 3)
			time += (float)delta;
			if (time >= RotationDuration)
			{
				Arrive();
				return;
			}

			Progress(time / RotationDuration);
		}

		/// <summary>
        /// Progress the rotation towards its endpoint
        /// </summary>
        /// <param name="x">[0, 1] progress along duration</param>
		protected virtual void Progress(float x)
		{
			Rotation = start.Rotation + ((target.Rotation - start.Rotation) * 6 * ((x * x / 2) - (x * x * x / 3)));

			AnchorLeft 	 = start.LeftAnchor   + (target.LeftAnchor   - start.LeftAnchor)   * x;
			AnchorRight  = start.RightAnchor  + (target.RightAnchor  - start.RightAnchor)  * x;
			AnchorTop 	 = start.TopAnchor	  + (target.TopAnchor 	 - start.TopAnchor)    * x;
			AnchorBottom = start.BottomAnchor + (target.BottomAnchor - start.BottomAnchor) * x;

			OffsetLeft 	 = start.LeftOffset   + (target.LeftOffset   - start.LeftOffset)   * x;
			OffsetRight  = start.RightOffset  + (target.RightOffset  - start.RightOffset)  * x;
			OffsetTop 	 = start.TopOffset	  + (target.TopOffset 	 - start.TopOffset)    * x;
			OffsetBottom = start.BottomOffset + (target.BottomOffset - start.BottomOffset) * x;
		}

		protected virtual void Arrive()
		{
			GD.Print($"Arrived at {target.Rotation}!");
			Rotation = target.Rotation;
			if (NormalizeAngleOnArrival)
			{
				while (Rotation > Math.PI) Rotation -= FullClockwiseRotation;
				while (Rotation < -Math.PI) Rotation += FullClockwiseRotation;
			}
		}

		public virtual void Resize()
		{
			PivotOffset = Size / 2;
			GD.Print($"Resizing. Size is {Size} and rotation is {Rotation}");
		}

		public void LookTowards(Vector2 targetPosition)
		{
			GD.Print($"{Name} looking towards {targetPosition}");
			RotateTowards(RotationForVector(targetPosition));
		}

		public void RotateTowards(float angle) => RotateTowards(start => start.With(rotation: angle));

		public void RotateTowards(Positioning target) => RotateTowards(start => target);

		public delegate Positioning From(Positioning start);

		public void RotateTowards(From from)
		{
			if (ArriveBeforeStartingNext) Arrive();

			start = Positioning.Of(this);
			target = from(start);
			GD.Print($"Rotating from {start} to {target}");
			time = 0f;
		}

		private float RotationForVector(Vector2 targetPosition)
			=> Mathf.Atan2(targetPosition.X - center.GlobalPosition.X,
						   center.GlobalPosition.Y - targetPosition.Y);
	}
}