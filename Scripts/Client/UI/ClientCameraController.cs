using Godot;
using Kompas.Client.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;
using System;

namespace Kompas.Client.UI
{
	public partial class ClientCameraController : Node3D
	{
		private const string CameraLeftActionName = "CameraLeft";
		private const string CameraRightActionName = "CameraRight";
		private const string CameraUpActionName = "CameraUp";
		private const string CameraDownActionName = "CameraDown";
		private static readonly Vector3 FriendlyHandRotation = (float)(-0.05 * Mathf.Pi) * Vector3.Right;

		[Export]
		private Camera3D? _camera;
		public Camera3D Camera => _camera
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _handObject;
		public Node3D HandObject => _handObject
			?? throw new UnassignedReferenceException();

		[Export]
		private float DistanceFromCamera { get; set; } = 0.35f;
		/// <summary>
        /// Horizontal plane (facing up), a configurable distance below the camera (DistanceFromCamera)
        /// </summary>
		public Plane AwayFromCamera => new(Vector3.Up, GlobalPosition + (DistanceFromCamera * Vector3.Down));
		/// <summary>
        /// Vertical plane (facing right) anchored at the camera's position
        /// </summary>
		public Plane CenterOfCamera => new(Vector3.Right, GlobalPosition);

		[Export]
		private Node3D? _boardCameraPosition;
		private Node3D BoardCameraPosition => _boardCameraPosition
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _friendlyDeckPosition;
		private Node3D FriendlyDeckPosition => _friendlyDeckPosition
			?? throw new UnassignedReferenceException();

		private CameraGraphNode? _currentPosition;
		private CameraGraphNode CurrentPosition => _currentPosition
			?? throw new NotReadyYetException();
		public override void _Ready()
		{
			var boardPosition = new CameraGraphNode(CameraPosition.Board, BoardCameraPosition);
			_currentPosition = boardPosition;

			var friendlyHandPosition = new CameraGraphNode(CameraPosition.FriendlyHand, BoardCameraPosition, FriendlyHandRotation);
			boardPosition.AddReciprocally(down: friendlyHandPosition);

			var friendlyDeckPosition = new CameraGraphNode(CameraPosition.FriendlyDeck, FriendlyDeckPosition);
			boardPosition.AddReciprocally(right: friendlyDeckPosition);
		}

		public override void _Process(double deltaTime)
		{
			if (Input.IsActionPressed(CameraRightActionName)) 		GoToCameraPosition(CurrentPosition.Right);
			else if (Input.IsActionPressed(CameraLeftActionName)) 	GoToCameraPosition(CurrentPosition.Left);
			else if (Input.IsActionPressed(CameraDownActionName)) 	GoToCameraPosition(CurrentPosition.Down);
			else if (Input.IsActionPressed(CameraUpActionName)) 	GoToCameraPosition(CurrentPosition.Up);
		}

		private void GoToCameraPosition(CameraGraphNode? node)
		{
			if (node == null) return;

			_currentPosition = node;
			GetParent()?.RemoveChild(this);
			node.Node.AddChild(this);
			Camera.Rotation = node.CameraRotation;
			Camera.Position = Vector3.Zero;
			GD.Print($"Setting {HandObject}'s rotation to {-node.CameraRotation}");
			HandObject.Rotation = new Vector3(Mathf.Pi / 2f, 0f, 0f) + node.CameraRotation;
		}

		public enum CameraPosition
		{
			Board,
			FriendlyHand,
			FriendlyDeck,
		}

		private class CameraGraphNode
		{
			public CameraPosition Position { get; }
			public Node3D Node { get; }
			public Vector3 CameraRotation { get; }

			//public setters because I might have some nodes I want to end up at the same endpoint
			public CameraGraphNode? Left { get; set; }
			public CameraGraphNode? Right { get; set; }
			public CameraGraphNode? Up { get; set; }
			public CameraGraphNode? Down { get; set; }

			public CameraGraphNode(CameraPosition position, Node3D node, Vector3? cameraRotation = null)
			{
				Position = position;
				Node = node;
				CameraRotation = cameraRotation ?? Vector3.Zero;
			}

			public void AddReciprocally(CameraGraphNode? left = null, CameraGraphNode? right = null,
				CameraGraphNode? up = null, CameraGraphNode? down = null)
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