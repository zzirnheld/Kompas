using Godot;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderSearchCardController : DeckBuilderCardController
	{
		private bool leftClickStayedOnThisCard = false;

		protected override void HandleMouseEvent(InputEventMouseButton mouseInput)
		{
			base.HandleMouseEvent(mouseInput);

			//The click released event will be attributed to this card even if the mouse is no longer on it
			if (mouseInput.ButtonIndex == MouseButton.Left) LeftClick(mouseInput.Pressed);
		}

		protected override void DoubleLeftClick() { }

		private void LeftClick(bool pressed)
		{
			_ = DeckController ?? throw new System.NullReferenceException("Forgot to init");
			_ = Card ?? throw new System.NullReferenceException("Forgot to init");
			// check if pressed and released on the same card TODO expand to have dragging capabilities
			if (pressed)
			{
				leftClickStayedOnThisCard = true;
			}
			else
			{
				if (leftClickStayedOnThisCard) DeckController.AddToDeck(Card);
				//regardless
				leftClickStayedOnThisCard = false;
			}
		}
	}
}