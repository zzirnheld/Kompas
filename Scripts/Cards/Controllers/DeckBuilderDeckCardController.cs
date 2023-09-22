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
				//TODO make left button able to drag? and shouldn't make char avatar at end of drag
				if (mouseInput.ButtonIndex == MouseButton.Left) { } //TODO avatar
				else if (mouseInput.ButtonIndex == MouseButton.Right) Delete();
			}
		}

		public void Delete()
		{
			DeckController.RemoveFromDeck(this);
			QueueFree();
		}
	}
}