using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Networking;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{
		public ClientCardRepository CardRepository { get; } = new ClientCardRepository();

		private ClientGame game;

		//TODO: aggressive nullable warning? encourage user to use null propagation?
		/// <summary>
        /// Singleton? which actually sends and receives communication.
        /// </summary>
		private ClientNetworker clientNetworker;
		/// <summary>
        /// Singleton? which assembles packets to be sent via the Networker.
        /// TODO consider changing the name to reflect this role
        /// </summary>
		private ClientNotifier clientNotifier;

		public override void _Ready()
		{
			base._Ready();
			game = ClientGame.Create(this);
		}

		private void SuccessfullyConnected(TcpClient tcpClient)
		{
			//ClientGame.clientUIController.connectionUIController.Show(ConnectionState.WaitingForPlayer);

			clientNetworker = new ClientNetworker(tcpClient, game);
			clientNotifier = new ClientNotifier(clientNetworker);
		}
	}
}