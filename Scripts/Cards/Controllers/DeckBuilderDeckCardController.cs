using Godot;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderDeckCardController : DeckBuilderCardController
	{

		protected override void LeftClickPressed() => GD.Print($"{Name} pressed");

		protected override void RightClicked()
		{
			DeckController.RemoveFromDeck(this);
			QueueFree();
		}
	}
}