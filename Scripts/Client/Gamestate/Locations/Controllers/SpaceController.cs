using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;

namespace Kompas.Client.Gamestate.Controllers
{
	public partial class SpaceController : Area3D
	{
		private const int BoardMax = 6;
		private static readonly Vector3 CardOffset = Vector3.Up * 0.002f;
		private static readonly Vector3 SpaceRotation = new(0f, 1.25f * Mathf.Pi, 0f);

		[Export]
		public int X { get; private set; }
		[Export]
		public int Y { get; private set; }

		[Export]
		private Node3D CanPlayTo { get; set; }

		[Export]
		private Label3D CoordsDebugLabel { get; set; }

		public event EventHandler LeftClick;

		public override void _Ready()
		{
			base._Ready();
			//CanPlayTo.Visible = false;
			InputEvent += HandleInputEvent;
		}

		public delegate bool ConfirmAdd(int x, int y);

		/// <summary>
        /// Creates a SpaceController that has position and coordinates corresponding to this SpaceController in some way.
        /// Has to ask for the prefab of itself because of Godot crust (which I understand, just find frustrating)
        /// </summary>
        /// <param name="parent">The Node that the new SpaceController should be parented to</param>
        /// <param name="spacePrefab">Because Duplicate() doesn't duplicate children and you can't add an [Export] variable that references itself</param>
        /// <param name="flipX">Whether to flip the X coordinate</param>
        /// <param name="flipY">Whether to flip the Y coordinate</param>
        /// <param name="swapXY">Whether to swap the X and Y coordinates (after flipping them, if applicable)</param>
        /// <param name="confirmAdd">Validation to perform once the coordinates have been determined (so as not to create, then abandon, new spaces.
        /// For some reason, just doing QueueFree on it didn't seem to clear them up. FUTURE: see if I can figure out another better way.)</param>
        /// <returns></returns>
		public SpaceController Dupe(Node3D parent, PackedScene spacePrefab, bool flipX, bool flipY, bool swapXY, ConfirmAdd confirmAdd)
		{
			(int tempX, int tempY) = (flipX ? BoardMax - X : X, flipY ? BoardMax - Y : Y);
			(int x, int y) = swapXY ? (tempY, tempX) : (tempX, tempY);

			if (!confirmAdd(x, y)) return null;

			//TODO duplicate was not recursive! or else it didn't move stuff properly. 
			SpaceController ret = spacePrefab.Instantiate() as SpaceController;
			ret.X = x;
			ret.Y = y;
			parent.AddChild(ret);

			(float tempXPos, float tempZPos) = (flipX ? -Position.X : Position.X, flipY ? -Position.Z : Position.Z);
			(float xPos, float zPos) = swapXY ? (tempZPos, tempXPos) : (tempXPos, tempZPos);
			ret.Position = new Vector3(xPos, Position.Y, zPos);
			ret.CoordsDebugLabel.Text = $"{ret.X}{ret.Y}{(flipX ? "Y" : "N")}{(flipY ? "Y" : "N")}{(swapXY ? "Y" : "N")}";

			ret.Rotation = SpaceRotation;

			return ret;
		}


		private void HandleInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx)
		{
			if (inputEvent is not InputEventMouseButton mouseEvent)
			{
				return;
			}

			//Event where now the mouseEvent is Pressed means it's when the mouse goes down
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				LeftClick?.Invoke(this, EventArgs.Empty);
			}
		}

		public void Place(ICardController card)
		{
			if (card.Node == null)
			{
				GD.PushWarning("Tried to place a card with a null Node!");
				return;
			}

			card.Node.GetParent()?.RemoveChild(card.Node);
			AddChild(card.Node);
			card.Node.Position = CardOffset;
			var rotation = card.Card.ControllingPlayer.Index * Mathf.Pi;
			card.Node.Rotation = new Vector3(0, rotation, 0);
		}
	}
}