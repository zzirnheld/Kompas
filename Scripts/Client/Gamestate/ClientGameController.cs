using System.Net.Sockets;
using Godot;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Networking;
using Kompas.Client.UI;
using Kompas.Client.UI.GameStart;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{
		public ClientCardRepository CardRepository { get; private set; }

		[Export]
		public GameStartController GameStartController { get; private set; }

		[Export]
		public ClientTargetingController TargetingController { get; private set; }

		[Export]
		public CurrentStateController CurrentStateController { get; private set; }

		[Export]
		private PackedScene CardPrefab { get; set; }

		private ClientGame game;
		public override IGame Game => game;

		//TODO: aggressive nullable warning? encourage user to use null propagation?
		/// <summary>
		/// Singleton? which actually sends and receives communication.
		/// </summary>
		public ClientNetworker Networker { get; private set; }
		/// <summary>
		/// Singleton? which assembles packets to be sent via the Networker.
		/// TODO consider changing the name to reflect this role
		/// </summary>
		public ClientNotifier Notifier { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			game = ClientGame.Create(this);
			CardRepository = new ClientCardRepository(CardPrefab);
			game.TurnChanged += (_, turnPlayer) => TurnStartOperations(turnPlayer);
		}

		private void TurnStartOperations(IPlayer turnPlayer)
			=> CurrentStateController.ChangeTurn(turnPlayer.Friendly);

		public override void _Process(double delta)
		{
			base._Process(delta);
			Networker?.Tick();
		}

		public void SuccessfullyConnected(TcpClient tcpClient)
		{
			Networker = new ClientNetworker(tcpClient, game);
			Notifier = new ClientNotifier(Networker);
		}
	}
}