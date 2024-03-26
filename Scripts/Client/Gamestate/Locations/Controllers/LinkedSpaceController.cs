using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.Gamestate.Controllers;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class LinkedSpaceController : Node3D
	{
		private static readonly Vector3 CardOffset = Vector3.Up * 0.002f;

		[Export]
		private SpaceTargetingController? _spaceTargetingController;
		public ISpaceTargetingController SpaceTargetingController => _spaceTargetingController
			?? throw new UnassignedReferenceException();

		public event EventHandler? LeftClick;
		
		[Export]
		private MeshInstance3D? _tile;
		private MeshInstance3D Tile => _tile ?? throw new UnassignedReferenceException();

		[Export]
		private MeshInstance3D? Plus1X { get; set; }
		[Export]
		private MeshInstance3D? Plus1Y { get; set; }
		[Export]
		private int x;
		[Export]
		private int y;

		public Space Coords => (x, y);
		
		public override void _Ready()
		{
			if (Plus1X == null && x < Space.BoardLen - 1) throw new UnassignedReferenceException();
			if (Plus1Y == null && y < Space.BoardLen - 1) throw new UnassignedReferenceException();
		}

		public void Display(bool plusX, bool plusY)
		{
			Visible = true;
			if (plusX)
			{
				if (Plus1X == null) throw new UnassignedReferenceException();
				Plus1X.Visible = true;
			}
			if (plusY)
			{
				if (Plus1Y == null) throw new UnassignedReferenceException();
				Plus1Y.Visible = true;
			}
		}

		public void DisplayNone()
		{
			Visible = false;
			if (Plus1X != null) Plus1X.Visible = false;
			if (Plus1Y != null) Plus1Y.Visible = false;
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
				Logger.Warn("Tried to place a card with a null Node!");
				return;
			}

			card.Node.GetParent()?.RemoveChild(card.Node);
			AddChild(card.Node);
			card.Node.Position = CardOffset;
			var rotation = card.Card.ControllingPlayer.Index * Mathf.Pi;
			card.Node.Rotation = new Vector3(0, rotation, 0);
			card.Node.Scale = Vector3.One;
		}

		public void ToggleHighlight(SpaceHighlight highlight, bool show) => SpaceTargetingController.ToggleHighlight(highlight, show);

		public void UpdateMaterial(Material material)
		{
			Tile.MaterialOverride = material;
			if (Plus1X != null) Plus1X.MaterialOverride = material;
			if (Plus1Y != null) Plus1Y.MaterialOverride = material;
		}

		public void UpdateTransparency(float t)
		{
			Tile.Transparency = t;
			if (Plus1X != null) Plus1X.Transparency = t;
			if (Plus1Y != null) Plus1Y.Transparency = t;
		}
	}
}