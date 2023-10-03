using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Networking;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate
{
	public partial class ClientGameController : GameController
	{
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

		public async Task AttemptConnect(string ip)
		{
			TcpClient tcpClient = null;
			try
			{
				tcpClient = await ClientNetworker.Connect(ip);
			}
			catch (System.Net.Sockets.SocketException e)
			{
				GD.PrintErr($"Failed to connect to {ip}. Stack trace:\n{e.StackTrace}");
				FailedToConnect();
				return;
			}

			if (tcpClient == null || !tcpClient.Connected) FailedToConnect();
			else SuccessfullyConnected(tcpClient);
		}

		private void FailedToConnect()
		{
			//TODO client connection workflow
			//ClientGame.clientUIController.connectionUIController.Show(ConnectionState.ChooseServer);
		}

		private void SuccessfullyConnected(TcpClient tcpClient)
		{
			//ClientGame.clientUIController.connectionUIController.Show(ConnectionState.WaitingForPlayer);

			clientNetworker = new ClientNetworker(tcpClient, game);
			clientNotifier = new ClientNotifier(clientNetworker);
		}
	}
}