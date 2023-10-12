using Godot;
using System;

namespace Kompas.Client.Cards.Controllers
{
	public partial class CardMouseController : Area3D
	{
		[Export]
		private ClientCardController CardController { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			MouseEntered += MouseOver;
			InputEvent += HandleInputEvent;
		}

		private void MouseOver() => CardController.ShowInTopLeft();

		private void HandleInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx)
		{
			if (inputEvent is not InputEventMouseButton mouseEvent) return;

			//Event where now the mouseEvent is Pressed means it's when the mouse goes down
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left) OnLeftMouseDown();
		}

		private void OnLeftMouseDown() => CardController.FocusInTopLeft();
	}
}