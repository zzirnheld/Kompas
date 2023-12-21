using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Effects.Views;
using Kompas.Client.Networking;
using Kompas.Client.UI;
using Kompas.Client.UI.GameStart;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{
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
		public UseEffectDialog? UseEffectDialog => _useEffectDialog ?? throw new UnassignedReferenceException();
		[Export]
		private ClientStackView? _stackView;
		public ClientStackView StackView => _stackView ?? throw new UnassignedReferenceException();

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

		public override void _Ready()
		{
			base._Ready();
			game = ClientGame.Create(this);
			game.TurnChanged += (_, turnPlayer) => TurnStartOperations(turnPlayer);
			_cardRespository = new ClientCardRepository(CardPrefab);
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