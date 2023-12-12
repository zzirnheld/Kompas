using Godot;
using Kompas.Client.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations;
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
		private static readonly Vector3 DefaultCameraParentRotation = new (Mathf.Pi / 2f, 0f, 0f);

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

		//TODO replace these with scripts attached to the objs that hold the up/down/left pointers?
		//or maybe instead have some sort of a "stack" notion, where if you hit "down" to go somewhere, hitting "up" means going the opposite way
		[Export]
		private Node3D? _boardCameraPosition;
		private Node3D BoardCameraPosition => _boardCameraPosition
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _friendlyDeckPosition;
		private Node3D FriendlyDeckPosition => _friendlyDeckPosition
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _friendlyDiscardPosition;
		private Node3D FriendlyDiscardPosition => _friendlyDiscardPosition
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _enemyHandPosition;
		private Node3D EnemyHandPosition => _enemyHandPosition
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _enemyDeckPosition;
		private Node3D EnemyDeckPosition => _enemyDeckPosition
			?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _enemyDiscardPosition;
		private Node3D EnemyDiscardPosition => _enemyDiscardPosition
			?? throw new UnassignedReferenceException();

		private CameraGraphNode? _currentPosition;
		private CameraGraphNode CurrentPosition => _currentPosition
			?? throw new NotReadyYetException();

		public readonly struct LookingAt
		{
			public Location Location { get; init; }
			public bool Friendly { get; init; }
		}

		public event EventHandler<LookingAt>? Departed;
		public event EventHandler<LookingAt>? Arrived;

		public override void _Ready()
		{
			var boardPosition = new CameraGraphNode(CameraPosition.Board, BoardCameraPosition,
				lookingAt: new() { Location = Location.Board });

			var friendlyHandPosition = new CameraGraphNode(CameraPosition.FriendlyHand, BoardCameraPosition,
				lookingAt: new() { Location = Location.Hand, Friendly = true },
				cameraRotation: FriendlyHandRotation);

			var friendlyDeckPosition = new CameraGraphNode(CameraPosition.FriendlyDeck, FriendlyDeckPosition,
				lookingAt: new() { Location = Location.Deck, Friendly = true });

			var friendlyDiscardPosition = new CameraGraphNode(CameraPosition.FriendlyDiscard, FriendlyDiscardPosition,
				lookingAt: new() { Location = Location.Discard, Friendly = true });

			var enemyHandPosition = new CameraGraphNode(CameraPosition.EnemyHand, EnemyHandPosition,
				lookingAt: new() { Location = Location.Hand, Friendly = false });

			var enemyDeckPosition = new CameraGraphNode(CameraPosition.EnemyDeck, EnemyDeckPosition,
				lookingAt: new() { Location = Location.Deck, Friendly = false });

			var enemyDiscardPosition = new CameraGraphNode(CameraPosition.EnemyDiscard, EnemyDiscardPosition,
				lookingAt: new() { Location = Location.Discard, Friendly = false });

			boardPosition.AddReciprocally(
				left: friendlyDiscardPosition,
				right: friendlyDeckPosition,
				up: enemyHandPosition,
				down: friendlyHandPosition
			);

			enemyHandPosition.AddReciprocally(
				left: enemyDeckPosition,
				right: enemyDiscardPosition
			);

			friendlyDeckPosition.AddReciprocally(up: enemyDiscardPosition);
			friendlyDiscardPosition.AddReciprocally(up: enemyDeckPosition);

			friendlyHandPosition.Left = friendlyDiscardPosition;
			friendlyHandPosition.Right = friendlyDeckPosition;

			_currentPosition = boardPosition;
		}

		public override void _Process(double deltaTime)
		{
			if (Input.IsActionJustReleased(CameraRightActionName)) 		GoToCameraPosition(CurrentPosition.Right);
			else if (Input.IsActionJustReleased(CameraLeftActionName)) 	GoToCameraPosition(CurrentPosition.Left);
			else if (Input.IsActionJustReleased(CameraDownActionName)) 	GoToCameraPosition(CurrentPosition.Down);
			else if (Input.IsActionJustReleased(CameraUpActionName)) 	GoToCameraPosition(CurrentPosition.Up);
		}

		private void GoToCameraPosition(CameraGraphNode? node)
		{
			if (node == null) return;

			if (_currentPosition != null) Departed?.Invoke(this, _currentPosition.LookingAt);
			_currentPosition = node;
			Arrived?.Invoke(this, node.LookingAt);

			GetParent()?.RemoveChild(this);
			node.Node.AddChild(this);

			Camera.Rotation = node.CameraRotation;
			Camera.Position = Vector3.Zero;
			HandObject.Rotation = DefaultCameraParentRotation + node.CameraRotation;
		}

		public enum CameraPosition
		{
			Board,
			FriendlyHand,
			FriendlyDeck,
			FriendlyDiscard,
			EnemyDeck,
			EnemyDiscard,
			EnemyHand,
		}

		private class CameraGraphNode
		{
			public CameraPosition Position { get; }
			public Node3D Node { get; }
			public Vector3 CameraRotation { get; }
			public LookingAt LookingAt { get; }

			//public setters because I might have some nodes I want to end up at the same endpoint
			public CameraGraphNode? Left { get; set; }
			public CameraGraphNode? Right { get; set; }
			public CameraGraphNode? Up { get; set; }
			public CameraGraphNode? Down { get; set; }


			public CameraGraphNode(CameraPosition position, Node3D node, LookingAt lookingAt, Vector3? cameraRotation = null)
			{
				Position = position;
				Node = node;
				LookingAt = lookingAt;
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