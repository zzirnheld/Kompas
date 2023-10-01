using System.Collections;
using Godot;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderDeckCardController : DeckBuilderCardController
	{
		private bool leftClickStayedOnThisCard = false;
		private bool rightClickStayedOnThisCard = false;

		protected override void HandleMouseEvent(InputEventMouseButton mouseInput)
		{
			base.HandleMouseEvent(mouseInput);

			if (!mouseInput.DoubleClick)
			{
				//The click released event will be attributed to this card even if the mouse is no longer on it
				if (mouseInput.ButtonIndex == MouseButton.Left) LeftClick(mouseInput.Pressed);
				else if (mouseInput.ButtonIndex == MouseButton.Right) RightClick(mouseInput.Pressed);
			}
		}

		private void LeftClick(bool pressed)
		{
			// check if pressed and released on the same card TODO expand to have dragging capabilities
			if (pressed)
			{
				leftClickStayedOnThisCard = true;
				DeckController.Dragging = this;
			}
			else
			{
				if (leftClickStayedOnThisCard) GD.Print($"Pressed and released on {Name}");

				//regardless
				DeckController.Dragging = null;
				leftClickStayedOnThisCard = false;
			}
		}

		private void RightClick(bool pressed)
		{
			if (pressed) rightClickStayedOnThisCard = true;
			else
			{
				if (rightClickStayedOnThisCard) Delete();

				//regardless
				rightClickStayedOnThisCard = false;
			}
		}

		public void MouseEnter() => DeckController.DragSwap(this);

		public void MouseExit() => leftClickStayedOnThisCard = rightClickStayedOnThisCard = false;

		public void Delete()
		{
			DeckController.RemoveFromDeck(this);
			QueueFree();
		}
	}
}