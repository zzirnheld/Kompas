using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class RotatingTextureRect : TextureRect
	{
		[Export]
		public Control center;

		public float RotationDuration { get; set; } = 0.5f;
		private float targetRotation;
		private float startRotation;
		private float time;
		//private float currentRotationalVelocity;

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

			float x = time / RotationDuration;
			Progress(x);
		}

		/// <summary>
        /// Progress the rotation towards its endpoint
        /// </summary>
        /// <param name="x">[0, 1] progress along duration</param>
		protected virtual void Progress(float x)
		{
			Rotation = startRotation + ((targetRotation - startRotation) * 6 * ((x * x / 2) - (x * x * x / 3)));
		}

		protected virtual void Arrive()
		{
			Rotation = targetRotation;
		}

		public void Resize()
		{
			GD.Print($"Resizing. Size is {Size}");
			PivotOffset = Size / 2;
		}

		public void LookTowards(Vector2 targetPosition)
		{
			RotateTowards(RotationForVector(targetPosition));
		}

		public void RotateTowards(float angle)
		{
			startRotation = Rotation;
			targetRotation = angle;
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