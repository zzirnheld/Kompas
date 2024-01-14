using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Kompas.Client.Gamestate;
using Kompas.Client.Networking;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.UI.GameStart
{
	public partial class GameStartController : Control
	{
		[Export]
		private ClientGameController? _gameController;
		public ClientGameController GameController => _gameController ?? throw new UnassignedReferenceException();

		[Export]
		private ConnectToServerController? _connectToServer;
		private ConnectToServerController ConnectToServer => _connectToServer ?? throw new UnassignedReferenceException();

		[Export]
		private Control? _waitingForServer;
		private Control WaitingForServer => _waitingForServer ?? throw new UnassignedReferenceException();
		[Export]
		private Control? _waitingForPlayer;
		private Control WaitingForPlayer => _waitingForPlayer ?? throw new UnassignedReferenceException();

		[Export]
		private SelectDeckController? _selectDeck;
		private SelectDeckController SelectDeck => _selectDeck ?? throw new UnassignedReferenceException();

		private enum State { ChooseHost, WaitingForServer, WaitingForPlayer, SelectDeck, DeckAccepted }
		private Dictionary<State, Control?> Tabs = new();

		private Task? connectionTask; //Awaited in TryConnect. not sure if this or a boolean is the better anti-reentrant mechanism

		public override void _Ready()
		{
			base._Ready();

			Tabs[State.ChooseHost] = ConnectToServer;
			Tabs[State.WaitingForServer] = WaitingForServer;
			Tabs[State.WaitingForPlayer] = WaitingForPlayer;
			Tabs[State.SelectDeck] = SelectDeck;
			Tabs[State.DeckAccepted] = null;

			foreach (State s in Enum.GetValues(typeof(State)))
			{
				if (!Tabs.ContainsKey(s)) GD.PrintErr($"No tab defined for game start state {s}");
			}

			ChangeState(State.ChooseHost);
		}

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
			TcpClient? tcpClient;
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

		public void GetDeck() => ChangeState(State.SelectDeck);
		public void DeckSubmitted() => ChangeState(State.WaitingForServer);

		public void DeckAccepted() => ChangeState(State.DeckAccepted);

		private void ChangeState(State state)
		{
			GD.Print($"Changing state to {state}");

			foreach (State s in Enum.GetValues(typeof(State)))
			{
				var tab = Tabs[s];
				if (tab != null) tab.Visible = s == state;
			}	

			if (state == State.DeckAccepted) Visible = false;
		}
	}
}