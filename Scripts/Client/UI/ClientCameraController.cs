using Godot;
using System;

namespace Kompas.Client.UI
{
	public partial class ClientCameraController : Node3D
	{
		private const string CameraLeftActionName = "CameraLeft";
		private const string CameraRightActionName = "CameraLeft";
		private const string CameraUpActionName = "CameraUp";
		private const string CameraDownActionName = "CameraDown";
		private static readonly Vector3 FriendlyHandRotation = (float)(-0.15 * Mathf.Pi) * Vector3.Right;

		[Export]
		public Camera3D Camera { get; private set; }

		[Export]
		private float DistanceFromCamera { get; set; } = 0.35f;
		public Plane AwayFromCamera => new(Vector3.Up, GlobalPosition + (DistanceFromCamera * Vector3.Down));
		public Plane CenterOfCamera => new(Vector3.Right, GlobalPosition);

		[Export]
		private Node3D BoardCameraPosition { get; set; }

		private CameraGraphNode currentPosition;
		public override void _Ready()
		{
			var boardPosition = new CameraGraphNode(CameraPosition.Board, BoardCameraPosition);
			currentPosition = boardPosition;

			var friendlyHandPosition = new CameraGraphNode(CameraPosition.FriendlyHand, BoardCameraPosition, FriendlyHandRotation);
			boardPosition.AddReciprocally(down: friendlyHandPosition);
		}

		public override void _Process(double deltaTime)
		{
			if (Input.IsActionPressed(CameraRightActionName)) 		GoToCameraPosition(currentPosition.Right);
			else if (Input.IsActionPressed(CameraLeftActionName)) 	GoToCameraPosition(currentPosition.Left);
			else if (Input.IsActionPressed(CameraDownActionName)) 	GoToCameraPosition(currentPosition.Down);
			else if (Input.IsActionPressed(CameraUpActionName)) 	GoToCameraPosition(currentPosition.Up);
		}

		private void GoToCameraPosition(CameraGraphNode node)
		{
			if (node == null) return;

			currentPosition = node;
			GetParent()?.RemoveChild(this);
			node.Node.AddChild(this);
			Camera.Rotation = node.CameraRotation;
		}

		public enum CameraPosition
		{
			Board,
			FriendlyHand,
		}

		private class CameraGraphNode
		{
			public CameraPosition Position { get; }
			public Node3D Node { get; }
			public Vector3 CameraRotation { get; }

			//public setters because I might have some nodes I want to end up at the same endpoint
			public CameraGraphNode Left { get; set; }
			public CameraGraphNode Right { get; set; }
			public CameraGraphNode Up { get; set; }
			public CameraGraphNode Down { get; set; }

			public CameraGraphNode(CameraPosition? position, Node3D node, Vector3? cameraRotation = null)
			{
				Position = position ?? throw new System.ArgumentException("CameraPosition name must not be null!", nameof(position));
				Node = node ?? throw new System.ArgumentException("CameraGraphNode must have a valid parent node to attach to!", nameof(node));
				CameraRotation = cameraRotation ?? Vector3.Zero;
			}

			public void AddReciprocally(CameraGraphNode left = null, CameraGraphNode right = null, CameraGraphNode up = null, CameraGraphNode down = null)
			{
				if (left != null)
				{
					Left = left;
					left.Right = this;
				}
				if (right != null)
				{
					Right = right;
					right.Left = this;
				}
				if (up != null)
				{
					Up = up;
					up.Down = this;
				}
				if (down != null)
				{
					Down = down;
					down.Up = this;
				}
			}
		}
	}
}