using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Controllers
{
	public partial class SpaceController : Area3D
	{
		private const int BoardMax = 6;
		private static readonly Vector3 CardOffset = Vector3.Up * 0.002f;

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

		//Assumes 
		public SpaceController Dupe(Node3D parent, bool flipX, bool flipY, bool swapXY, ConfirmAdd confirmAdd, PackedScene self)
		{
			var (tempX, tempY) = (flipX ? BoardMax - X : X, 
						  flipY ? BoardMax - Y : Y);
			var (x, y) = swapXY ? (tempY, tempX) : (tempX, tempY);

			if (!confirmAdd(x, y)) return null;

			//TODO duplicate was not recursive! or else it didn't move stuff properly. 
			var ret = self.Instantiate() as SpaceController;
			ret.X = x;
			ret.Y = y;
			parent.AddChild(ret);

			var (tempXPos, tempZPos) = (flipX ? -Position.X : Position.X,
										flipY ? -Position.Z : Position.Z);
			var (xPos, zPos) = swapXY ? (tempZPos, tempXPos) : (tempXPos, tempZPos);
			GD.Print($"{X}{Y} at {Position} + {(flipX ? "Y" : "N")}{(flipY ? "Y" : "N")}{(swapXY ? "Y" : "N")} becomes {ret.X},{ret.Y} at {xPos}, {zPos}");
			ret.Position = new Vector3(xPos, Position.Y, zPos);
			ret.CoordsDebugLabel.Text = $"{ret.X}{ret.Y}{(flipX ? "Y" : "N")}{(flipY ? "Y" : "N")}{(swapXY ? "Y" : "N")}";
			ret.LeftClick += (_, _) => GD.Print($"According to creator, {ret.X}{ret.Y}{(flipX ? "Y" : "N")}{(flipY ? "Y" : "N")}{(swapXY ? "Y" : "N")}");

			return ret;
		}


		private void HandleInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx)
		{
			if (inputEvent is not InputEventMouseButton mouseEvent) return;

			//Event where now the mouseEvent is Pressed means it's when the mouse goes down
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				LeftClick?.Invoke(this, EventArgs.Empty);
				GD.Print($"{X}, {Y} at {Position}");
			} 
		}

		public void Place(ICardController card)
		{
			if (card.Node == null)
			{
				GD.PushWarning("Tried to place a card with a null Node!");
				return;
			}

			AddChild(card.Node);
			card.Node.Position = CardOffset;
		}
	}
}