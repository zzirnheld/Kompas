using Godot;
using System;

namespace Kompas.Client.UI
{
	public partial class ClientCameraController : Camera3D
	{
		[Export]
		private float DistanceFromCamera { get; set; } = 0.25f;
		public Plane AwayFromCamera => new(Vector3.Up, GlobalPosition + (DistanceFromCamera * Vector3.Down));
		public Plane CenterOfCamera => new(Vector3.Right, GlobalPosition);

		private Vector3 startingPosition;
		public override void _Ready()
		{
			startingPosition = Position;
		}

		private double time = 0;
		public override void _Process(double deltaTime)
		{
			time += deltaTime;
			//Position = startingPosition + (Vector3.Forward * 0.1f * (float) Mathf.Sin(time));
		}
	}
}