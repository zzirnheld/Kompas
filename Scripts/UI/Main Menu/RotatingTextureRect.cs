using System;
using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class RotatingTextureRect : TextureRect
	{
		private const float FullClockwiseRotation = (float)(2f * Math.PI);

		[Export]
		public Control center;

		protected virtual float InitialRotation => 0f;

		protected virtual float RotationDuration => 0.5f;

		protected struct Positioning
		{
			public float rotation;
			public float leftAnchor;
			public float rightAnchor;
			public float topAnchor;
			public float bottomAnchor;

			public static Positioning Of(Control obj)
			{
				return new()
				{
					rotation = obj.Rotation,
					leftAnchor = obj.AnchorLeft,
					rightAnchor = obj.AnchorRight,
					topAnchor = obj.AnchorTop,
					bottomAnchor = obj.AnchorBottom,
				};
			}

			public Positioning With(float? rotation, float? leftAnchor, float? rightAnchor, float? topAnchor, float? bottomAnchor)
			{
				return new()
				{
					rotation = rotation ?? this.rotation,
					leftAnchor = leftAnchor ?? this.leftAnchor,
					rightAnchor = rightAnchor ?? this.rightAnchor,
					topAnchor = topAnchor ?? this.topAnchor,
					bottomAnchor = bottomAnchor ?? this.bottomAnchor,
				};
			}
		}
		protected Positioning target = new();

		private Positioning start;
		private float time;

		protected virtual bool ArriveBeforeStartingNext => false;
		protected virtual bool NormalizeAngleOnArrival => true;
		//private float currentRotationalVelocity;

		public override void _Ready()
		{
			Rotation = target.rotation = start.rotation = InitialRotation;
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
			Rotation = start.rotation + ((target.rotation - start.rotation) * 6 * ((x * x / 2) - (x * x * x / 3)));
			AnchorLeft = start.leftAnchor + (target.leftAnchor - start.leftAnchor) * x;
			AnchorRight = start.rightAnchor + (target.rightAnchor - start.rightAnchor) * x;
			AnchorTop = start.topAnchor + (target.topAnchor - start.topAnchor) * x;
			AnchorBottom = start.bottomAnchor + (target.bottomAnchor - start.bottomAnchor) * x;
		}

		protected virtual void Arrive()
		{
			GD.Print($"Arrived at {target.rotation}!");
			Rotation = target.rotation;
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

		public virtual void RotateTowards(float angle,
			float? targetLeftAnchor = null, float? targetRightAnchor = null,
			float? targetTopAnchor = null, float? targetBottomAnchor = null)
		{
			if (ArriveBeforeStartingNext) Arrive();
			GD.Print($"Rotating from {Rotation} to {angle}");
			start = Positioning.Of(this);
			target = start.With(angle, targetLeftAnchor, targetRightAnchor, targetTopAnchor, targetBottomAnchor);
			//GD.Print($"from {currentPosition} to {targetPosition}, {targetPosition.X - currentPosition.X} , {currentPosition.Y - targetPosition.Y}, so target rotation {targetRotation}");
			time = 0f;
		}

		private float RotationForVector(Vector2 targetPosition)
		{
			var currentPosition = center.GlobalPosition;
			return Mathf.Atan2(targetPosition.X - currentPosition.X,
							   currentPosition.Y - targetPosition.Y);
		}
	}
}