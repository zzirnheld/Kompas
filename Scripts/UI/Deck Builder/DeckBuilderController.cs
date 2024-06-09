using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.Client.UI;
using Kompas.Godot;
using Kompas.Shared;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;
using Kompas.UI.MainMenu;

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

		public override async void _Ready()
		{
			CardView.Refresh();
			EscapeMenu.Init(
				new EscapeMenuController.ButtonData() { Text = "Back to\nMain Menu", OnClick = () => ToMainMenu() }
			);

			Task load = DeckController.Init();

			await Task.WhenAny(
				EscapeMenu.SpinForTransitionWithMainMenu(LogoSpinController.SpinDirection.Clockwise), //Instant, no delay
				load
			);

			await EscapeMenu.CameFromMainMenuClose();
		}

		private async Task SwitchSceneTo(string scenePath)
		{
			ResourceLoader.LoadThreadedRequest(scenePath);
			Task<bool> load = this.DoEachFrame(_ =>
			{
				var status = ResourceLoader.LoadThreadedGetStatus(scenePath);
				if (status == ResourceLoader.ThreadLoadStatus.InProgress)
					return Result<bool>.None;

				return Result<bool>.Of(status == ResourceLoader.ThreadLoadStatus.Loaded);
			});

			await EscapeMenu.PrepareForGoingToMainMenu(0.5f);
			await Task.WhenAny(load, EscapeMenu.SpinForTransitionWithMainMenu(LogoSpinController.SpinDirection.CounterClockwise));

			if (load.IsCompleted && load.Result)
			{
				var res = ResourceLoader.LoadThreadedGet(scenePath);
				if (res is not PackedScene scene) throw new System.InvalidOperationException("Resource was not a packed scene!");
				GetTree().ChangeSceneToPacked(scene);
			}
		}

		private async void ToMainMenu() => await SwitchSceneTo(MainMenuPath);
	}
}
