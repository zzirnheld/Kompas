
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
		private PackedScene CardPrefab { get; set; }

		private ServerCardRepository CardRepo { get; set; }

		private TcpListener listener;
		private readonly IList<ServerGameController> games = new List<ServerGameController>();
		private TcpClient currentlyWaitingTcpClient = null;
		private Task<TcpClient> currTcpClient;

		public override void _Ready()
		{
			try { listener = new TcpListener(IPAddress.Any, Networker.port); }
			catch (System.Exception e)
			{
				GD.PrintErr(e.Message);
				return;
			}

			listener.Start();
			currTcpClient = listener.AcceptTcpClientAsync();
			CardRepo = new ServerCardRepository(CardPrefab);
		}

		public override void _Process(double delta)
		{
			if (currTcpClient.IsCompleted)
			{
				var client = currTcpClient.Result;

				if (currentlyWaitingTcpClient == null) currentlyWaitingTcpClient = client;
				else
				{
					var gameController = GamePrefab.Instantiate() as ServerGameController
						?? throw new System.NotSupportedException("Server Game prefab wasn't a ServerGameController!");
					AddChild(gameController);
					gameController.Init(new TcpClient[] { currentlyWaitingTcpClient, client }, CardRepo);
					games.Add(gameController);
					currentlyWaitingTcpClient = null;
				}

				currTcpClient = listener.AcceptTcpClientAsync();
			}
		}
	}
}