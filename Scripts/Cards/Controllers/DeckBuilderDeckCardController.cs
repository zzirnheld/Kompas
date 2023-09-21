using Godot;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderDeckCardController : DeckBuilderCardController
	{
		public void Clicked(InputEvent input)
		{
			//If clicked, remove
			if (input is InputEventMouseButton mouseInput && mouseInput.Pressed)
			{
				if (mouseInput.ButtonIndex == MouseButton.Left) Delete();
				else if (mouseInput.ButtonIndex == MouseButton.Right)
				{
					//TODO something probably. become avatar? duplicate?
				}
			}
		}

		public void Delete()
		{
			DeckController.RemoveFromDeck(this);
			QueueFree();
		}
	}
}