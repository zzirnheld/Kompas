using Godot;
using System;

namespace Kompas.Cards.Controllers
{
	public partial class CardMouseController : Area3D
	{
		public event EventHandler? HoverBegin;
		public event EventHandler? HoverEnd;
		/// <summary>
		/// Argument: Whether the click was a double click
		/// </summary>
		public event EventHandler<bool>? LeftClick;
		/// <summary>
        /// Argument: Whether the click was a double click
        /// </summary>
		public event EventHandler<bool>? RightClick;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			MouseEntered += () => HoverBegin?.Invoke(this, EventArgs.Empty);
			MouseExited += () => HoverEnd?.Invoke(this, EventArgs.Empty);
			InputEvent += HandleInputEvent;
		}

		private void HandleInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx)
		{
			if (inputEvent is not InputEventMouseButton mouseEvent) return;

			//Event where now the mouseEvent is Pressed means it's when the mouse goes down
			if (mouseEvent.Pressed)
			{
				if (mouseEvent.ButtonIndex == MouseButton.Left) LeftClick?.Invoke(this, mouseEvent.DoubleClick);
				if (mouseEvent.ButtonIndex == MouseButton.Right) RightClick?.Invoke(this, mouseEvent.DoubleClick);
			}
		}
	}
}