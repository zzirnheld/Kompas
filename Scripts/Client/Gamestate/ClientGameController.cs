using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Networking;
using Kompas.Client.UI.GameStart;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{
		public ClientCardRepository CardRepository { get; } = new ClientCardRepository();

		[Export]
		public GameStartController GameStartController { get; private set; }

		private ClientGame game;
		public override IGame Game => game;

		//TODO: aggressive nullable warning? encourage user to use null propagation?
		/// <summary>
		/// Singleton? which actually sends and receives communication.


		//TODO: aggressive nullable warning? encourage user to use null propagation?
		/// <summary>
		/// Singleton? which actually sends and receives communication.
		/// </summary>
		private ClientNetworker networker;
		/// <summary>
        /// Singleton? which assembles packets to be sent via the Networker.
        /// TODO consider changing the name to reflect this role
        /// </summary>
		public ClientNotifier Notifier { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			game = ClientGame.Create(this);
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			networker?.Tick();
		}

		public void SuccessfullyConnected(TcpClient tcpClient)
		{
			networker = new ClientNetworker(tcpClient, game);
			Notifier = new ClientNotifier(networker);
		}
	}
}