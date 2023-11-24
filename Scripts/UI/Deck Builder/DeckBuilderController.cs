using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderController : Control
	{
		private const string MainMenuPath = "res://Scenes/MainMenuScene.tscn";

		[Export]
		private ControlInfoDisplayer? _cardInfoDisplayer;
		private ControlInfoDisplayer CardInfoDisplayer => _cardInfoDisplayer
			?? throw new UnassignedReferenceException();
		[Export]
		private DeckBuilderDeckController? _deckController;
		public DeckBuilderDeckController DeckController => _deckController
			?? throw new UnassignedReferenceException();

		public DeckBuilderCardRepository CardRepository { get; } = new DeckBuilderCardRepository();

		private DeckBuilderTopLeftCardView? cardView;
		public DeckBuilderTopLeftCardView CardView => cardView ??= new DeckBuilderTopLeftCardView(CardInfoDisplayer);

		public override void _Ready()
		{
			CardView.Refresh();
		}

		private void ToMainMenu() => GetTree().ChangeSceneToFile(MainMenuPath);
	}
}
