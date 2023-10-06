using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Networking;

namespace Kompas.Client.UI.GameStart
{
	public partial class GameStartController : Control
	{
		[Export]
		private ConnectToServerController ConnectToServer { get; set; }

		[Export]
		private Control WaitingForServer { get; set; }
		[Export]
		private Control WaitingForPlayer { get; set; }

		[Export]
		private SelectDeckController SelectDeck { get; set; }

		private Task connectionTask; //Can't be awaited because Godot doesn't let you modify 

		public override void _Process(double delta)
		{
			base._Process(delta);
			if (connectionTask?.IsCompleted ?? false)
			{
				connectionTask = null;
			}
		}

		public void TryConnect(string ip)
		{
			if (connectionTask != null)
			{
				GD.Print("Already connecting");
				return;
			}

			connectionTask = Connect(ip);
		}

		public async Task Connect(string ip)
		{
			TcpClient tcpClient;
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
			GD.Print("Failed!");
			WaitingForPlayer.Visible = false;

			//TODO client connection workflow
			//ClientGame.clientUIController.connectionUIController.Show(ConnectionState.ChooseServer);
		}

		private void SuccessfullyConnected(TcpClient tcpClient)
		{
			GD.Print("Succeeded!");
			WaitingForPlayer.Visible = true;

			//TODO tell GameStartController to new up the connections

			//TODO function for deck select
		}
	}
}