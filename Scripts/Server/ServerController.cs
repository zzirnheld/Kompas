
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Networking;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Networking
{
	public partial class ServerController : Node
	{
		[Export]
		private PackedScene GamePrefab { get; set; }
		[Export]
		private ServerCardRepository CardRepo { get; set; }

		private TcpListener listener;
		private readonly IList<ServerGameController> games = new List<ServerGameController>();
		private TcpClient currentlyWaitingPlayer = null;
		private Task<TcpClient> currTcpClient;

		public override void _Ready()
		{
			try { listener = new TcpListener(IPAddress.Any, NetworkController.port); }
			catch (System.Exception e)
			{
				GD.PrintErr(e.Message);
				return;
			}

			listener.Start();
			currTcpClient = listener.AcceptTcpClientAsync();
		}

		public override void _Process(double delta)
		{
			if (currTcpClient.IsCompleted)
			{
				var client = currTcpClient.Result;

				if (currentlyWaitingPlayer == null) currentlyWaitingPlayer = client;
				else
				{
					var gameController = GamePrefab.Instantiate() as ServerGameController ?? throw new System.NotSupportedException();
					//TODO init curr game?
					games.Add(gameController);
					currentlyWaitingPlayer = null;
				}

				currTcpClient = listener.AcceptTcpClientAsync();
			}
		}
	}
}