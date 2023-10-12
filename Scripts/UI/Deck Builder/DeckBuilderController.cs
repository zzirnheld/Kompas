using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderController : Control
	{
		private const string MainMenuPath = "res://Scenes/MainMenuScene.tscn";

		[Export]
		private ControlInfoDisplayer CardInfoDisplayer { get; set; }
		[Export]
		public DeckBuilderDeckController DeckController { get; private set; }

		public DeckBuilderCardRepository CardRepository { get; } = new DeckBuilderCardRepository();

		private DeckBuilderTopLeftCardView cardView;
		public DeckBuilderTopLeftCardView CardView => cardView ??= new DeckBuilderTopLeftCardView(CardInfoDisplayer);

		public override void _Ready()
		{
			CardView.Refresh();
		}

		private void ToMainMenu() => GetTree().ChangeSceneToFile(MainMenuPath);
	}
}
