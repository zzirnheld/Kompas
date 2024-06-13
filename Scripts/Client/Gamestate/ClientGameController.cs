using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Effects.Controllers;
using Kompas.Client.Effects.Views;
using Kompas.Client.Networking;
using Kompas.Client.UI;
using Kompas.Client.UI.GameStart;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Godot;
using Kompas.Shared;
using Kompas.Shared.Controllers;
using Kompas.Shared.Exceptions;
using Kompas.UI.MainMenu;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{
		private const string RematchPath = "res://Scenes/ClientScene.tscn";
		private const string MainMenuPath = "res://Scenes/MainMenuScene.tscn";

		private ClientCardRepository? _cardRespository;
		public ClientCardRepository CardRepository => _cardRespository
			?? throw new NotInitializedException();

		[Export]
		private GameStartController? _gameStartController;
		public GameStartController GameStartController => _gameStartController ?? throw new UnassignedReferenceException();
		[Export]
		private ClientTargetingController? _targetingController;
		public ClientTargetingController TargetingController => _targetingController ?? throw new UnassignedReferenceException();
		[Export]
		private CurrentStateController? _currentStateController;
		public CurrentStateController CurrentStateController => _currentStateController ?? throw new UnassignedReferenceException();
		[Export]
		private UseEffectDialog? _useEffectDialog;
		public UseEffectDialog UseEffectDialog => _useEffectDialog ?? throw new UnassignedReferenceException();
		[Export]
		private ClientStackView? _stackView;
		public ClientStackView StackView => _stackView ?? throw new UnassignedReferenceException();
		[Export]
		private ClientCameraController? _camera;
		public ClientCameraController Camera => _camera ?? throw new UnassignedReferenceException();
		[Export]
		private ClientChoicesView? _choicesView;
		private ClientChoicesView ChoicesView => _choicesView ?? throw new UnassignedReferenceException();
		[Export]
		private EscapeMenuController? _escapeMenu;
		private EscapeMenuController EscapeMenu => _escapeMenu ?? throw new UnassignedReferenceException(nameof(_escapeMenu));

		[Export]
		private PackedScene? _cardPrefab;
		private PackedScene CardPrefab => _cardPrefab ?? throw new UnassignedReferenceException();

		private ClientGame? game;
		public override IGame Game => game ?? throw new NotReadyYetException();

		//TODO: aggressive nullable warning? encourage user to use null propagation?
		/// <summary>
		/// Singleton? which actually sends and receives communication.
		/// </summary>
		public ClientNetworker? Networker { get; private set; }
		private ClientNotifier? _notifier;
		/// <summary>
		/// Singleton? which assembles packets to be sent via the Networker.
		/// TODO consider changing the name to reflect this role
		/// </summary>
		public ClientNotifier Notifier => _notifier ?? throw new NotReadyYetException();

		private ClientChoicesController? _choices;
		public ClientChoicesController Choices => _choices ?? throw new NotReadyYetException();

		public override async void _Ready()
		{
			base._Ready();
			game = ClientGame.Create(this);
			game.TurnChanged += (_, turnPlayer) => TurnStartOperations(turnPlayer);
			_cardRespository = new ClientCardRepository(CardPrefab);

			_choices = new ClientChoicesController(ChoicesView);
			Choices.ChooseIndex += (_, index) => Notifier.RequestChooseEffectOption(index);

			EscapeMenu.Init(
				new EscapeMenuController.ButtonData() { Text = "Rematch", OnClick = Rematch },
				new EscapeMenuController.ButtonData() { Text = "Main Menu", OnClick = ToMainMenu }
			);

			//Spin to match that we came from the main menu,
			//But really we're awaiting the initialization of the select deck view
			await Task.WhenAny(
				EscapeMenu.SpinForTransitionWithMainMenu(LogoSpinController.SpinDirection.Clockwise),
				GameStartController.SelectDeck.Init()
			);

			//Once select deck has been initialized, move to close the spinner
			await EscapeMenu.CameFromMainMenuClose();

			//TODO add event to game started that should close the load window
			game.GameStarted += (_, _) => GameStartController.Hide();
		}

		private async void Rematch() => await SwitchSceneTo(RematchPath);
		private async void ToMainMenu() => await SwitchSceneTo(MainMenuPath);
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

			Task loadingScreenOngoing = EscapeMenu.SpinForTransitionWithMainMenu(LogoSpinController.SpinDirection.CounterClockwise);
			await Task.WhenAny(load, loadingScreenOngoing);

			if (load.IsCompleted && load.Result)
			{
				var res = ResourceLoader.LoadThreadedGet(scenePath);
				if (res is not PackedScene scene) throw new System.InvalidOperationException("Resource was not a packed scene!");
				GetTree().ChangeSceneToPacked(scene);
			}
		}

		public override void _Input(InputEvent inputEvent)
		{
			//if (inputEvent is InputEventKey keyEvent && keyEvent.Keycode == Key.Escape && !keyEvent.Pressed) GetTree().Quit();
		}

		private void TurnStartOperations(IPlayer turnPlayer)
		{
			_ = CurrentStateController ?? throw new System.NullReferenceException("Failed to initialize");
			CurrentStateController.ChangeTurn(turnPlayer.Friendly);
		}

		//Remember, async voids don't get awaited.
		//This means that Process will get called again before this call completes,
		//if and only if networker.Tick returns an incomplete Task (i.e. calls something else)
		public override async void _Process(double delta)
		{
			base._Process(delta);
			if (Networker != null) await Networker.Tick();
		}

		public void SuccessfullyConnected(TcpClient tcpClient)
		{
			_ = game ?? throw new System.NullReferenceException("Not ready yet");
			Networker = new ClientNetworker(tcpClient, game);
			_notifier = new ClientNotifier(Networker);
		}
	}
}