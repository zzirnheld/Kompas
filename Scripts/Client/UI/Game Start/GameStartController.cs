using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Kompas.Client.Gamestate;
using Kompas.Client.Networking;

namespace Kompas.Client.UI.GameStart
{
	public partial class GameStartController : Control
	{
		[Export]
		public ClientGameController GameController { get; private set; }

		[Export]
		private ConnectToServerController ConnectToServer { get; set; }

		[Export]
		private Control WaitingForServer { get; set; }
		[Export]
		private Control WaitingForPlayer { get; set; }

		[Export]
		private SelectDeckController SelectDeck { get; set; }

		private enum State { ChooseHost, WaitingForServer, WaitingForPlayer, SelectDeck }
		private Dictionary<State, Control> Tabs = new();

		private Task connectionTask; //Awaited in TryConnect. not sure if this or a boolean is the better anti-reentrant mechanism

		public override void _Ready()
		{
			base._Ready();

			Tabs[State.ChooseHost] = ConnectToServer;
			Tabs[State.WaitingForServer] = WaitingForServer;
			Tabs[State.WaitingForPlayer] = WaitingForPlayer;
			Tabs[State.SelectDeck] = SelectDeck;

			foreach (State s in Enum.GetValues(typeof(State)))
			{
				if (!Tabs.ContainsKey(s)) GD.PrintErr($"No tab defined for game start state {s}");
			}

			ChangeState(State.ChooseHost);
		}

		public void GetDeck() => ChangeState(State.SelectDeck);

		/// <summary>
		/// Tries to connect to the given IP.
		/// Doesn't allow trying to connect while you're already trying to connect.
		/// Fire and forget. Async void isn't great but I don't want to hang the app on a button press by awaiting it in Process
		/// </summary>
		/// <param name="ip"></param>
		public async void TryConnect(string ip)
		{
			if (connectionTask != null)
			{
				GD.Print("Already trying to connect!");
				return;
			}

			connectionTask = Connect(ip);
			await connectionTask;
			connectionTask = null;
		}

		private async Task Connect(string ip)
		{
			ChangeState(State.WaitingForServer);
			TcpClient tcpClient;
			try
			{
				tcpClient = await ClientNetworker.Connect(ip);
			}
			catch (SocketException e)
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

			ChangeState(State.ChooseHost);
		}

		private void SuccessfullyConnected(TcpClient tcpClient)
		{
			GD.Print("Succeeded!");

			ChangeState(State.WaitingForPlayer);
			GameController.SuccessfullyConnected(tcpClient);
		}

		private void ChangeState(State state)
		{
			GD.Print($"Changing state to {state}");

			foreach (State s in Enum.GetValues(typeof(State)))
				Tabs[s].Visible = s == state;
		}
	}
}