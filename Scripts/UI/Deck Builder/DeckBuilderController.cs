using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.Client.UI;
using Kompas.Shared.Controllers;
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
		[Export]
		private ReminderTextPopup? _reminderTextPopup;
		public ReminderTextPopup ReminderTextPopup => _reminderTextPopup
			?? throw new UnassignedReferenceException();

		[Export]
		private EscapeMenuController? _escapeMenu;
		private EscapeMenuController EscapeMenu => _escapeMenu
			?? throw new UnassignedReferenceException(nameof(_escapeMenu));

		public DeckBuilderCardRepository CardRepository { get; } = new DeckBuilderCardRepository();

		private DeckBuilderTopLeftCardView? cardView;
		public DeckBuilderTopLeftCardView CardView => cardView ??= new DeckBuilderTopLeftCardView(CardInfoDisplayer, ReminderTextPopup, CardRepository);

		public override void _Ready()
		{
			CardView.Refresh();
			EscapeMenu.Init(
				new EscapeMenuController.ButtonData() { Text = "Back to\nMain Menu", OnClick = () => ToMainMenu() }
			);
		}

		private void ToMainMenu() => GetTree().ChangeSceneToFile(MainMenuPath);
	}
}
