using Godot;
using System;

namespace Kompas.Cards.Controllers
{
	public partial class CardMouseController : Area3D
	{
		public event EventHandler MouseOver;
		public event EventHandler LeftClick;
		public event EventHandler RightClick;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			MouseEntered += () => MouseOver?.Invoke(this, EventArgs.Empty);
			InputEvent += HandleInputEvent;
		}

		private void HandleInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx)
		{
			if (inputEvent is not InputEventMouseButton mouseEvent) return;

			//Event where now the mouseEvent is Pressed means it's when the mouse goes down
			if (mouseEvent.Pressed)
			{
				if (mouseEvent.ButtonIndex == MouseButton.Left) LeftClick?.Invoke(this, EventArgs.Empty);
				if (mouseEvent.ButtonIndex == MouseButton.Right) RightClick?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}