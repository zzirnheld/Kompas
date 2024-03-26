using System;
using System.Linq;
using Godot;
using Kompas.Client.UI;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientHandController : HandController
	{
		private const int FrustumLeft = 2;
		private const int FrustumRight = 4;
		private const int FrustumBottom = 5;

		private const float CardOffset = 1.125f;
		private const float HandWidthProportion = 5f / 9f;
		/// <summary>
		/// A minimum number of cards we must have in hand to start scaling according to that number of cards in hand.
		/// We don't want to blow up a single card to cover the entire middle of the screen, for example.
		/// </summary>
		private const int MinHandCountForScale = 5;

		[Export]
		private ClientCameraController? _camera;
		private ClientCameraController? Camera => _camera; // ?? throw new UnassignedReferenceException();
		[Export]
		private Node3D? _nodeParent;
		private Node3D NodeParent => _nodeParent ?? throw new UnassignedReferenceException();
		[Export]
		private Node3D? _leftBound;
		private Node3D LeftBound => _leftBound ?? throw new UnassignedReferenceException();
		[Export]
		private Node3D? _rightBound;
		private Node3D RightBound => _rightBound ?? throw new UnassignedReferenceException();
		public override void _Ready() => Recenter();

		private float handWidth;

		/// <summary>
		/// Figures out where the center of the camera window is,
		/// and places the center of the hand + the left and right bounds accordingly.
		/// Currently only does this on startup.
		/// (Since we then thereafter move around the hand rotation/position relative to the camera, will have to figure out if this is exactly what I want)
		/// </summary>
		public void Recenter()
		{
			if (Camera == null) return;

			var frustums = Camera.Camera.GetFrustum();
			//NOTE: AwayFromCamera and CenterOfCamera assume the camera is facing directly down.
			//If we call Recenter more often, we may need to adjust that (with some like, transformation of forward or right)
			Plane distanceFromCamera = Camera.AwayFromCamera;

			NodeParent.GlobalPosition 	= frustums[FrustumBottom].Intersect3(distanceFromCamera, Camera.CenterOfCamera) ?? Vector3.Zero;
			LeftBound.GlobalPosition 	= frustums[FrustumBottom].Intersect3(distanceFromCamera, frustums[FrustumLeft]) ?? Vector3.Zero;
			RightBound.GlobalPosition 	= frustums[FrustumBottom].Intersect3(distanceFromCamera, frustums[FrustumRight]) ?? Vector3.Zero;

			handWidth = (LeftBound.GlobalPosition - RightBound.GlobalPosition).Length();
		}

		protected override void SpreadAllCards()
		{
			float scale = (handWidth * HandWidthProportion) / (CardOffset * Math.Max(HandModel.HandSize, MinHandCountForScale));
			NodeParent.Scale = scale * Vector3.One;
			Logger.Log($"{handWidth} * {HandWidthProportion} / {CardOffset} * {HandModel.HandSize} = {NodeParent.Scale.X}");
			Logger.Log($"Spreading {HandModel.HandSize} = {HandModel.Cards.Count()} cards: {string.Join(", ", HandModel.Cards.Select(c => c.CardName))}");
			for (int i = 0; i < HandModel.HandSize; i++)
			{
				var node = HandModel[i].CardController.Node;
				node.GetParent()?.RemoveChild(node);
				NodeParent.AddChild(node);

				//Offset = (card's index) - (1/2 * # cards in hand)
				float offsetMultiplier = i - (HandModel.HandSize / 2f) + 0.5f;
				node.Position = Vector3.Right * (CardOffset * offsetMultiplier);
				
				node.Rotation = Vector3.Zero;
				node.Visible = true;
				Logger.Log($"Placing {HandModel[i].CardName} at {node.Position}");
			}
		}
	}
}