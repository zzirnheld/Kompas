
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Networking;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Server.Networking
{
	public partial class ServerController : Node
	{
		[Export]
		private PackedScene? _gamePrefab;
		private PackedScene GamePrefab => _gamePrefab
			?? throw new UnassignedReferenceException();

		[Export]
		private CheckBox? _debugMode;
		public bool DebugMode => _debugMode?.ButtonPressed
			?? throw new UnassignedReferenceException();

		private ServerCardRepository? CardRepo { get; set; }

		private TcpListener? listener;
		private readonly IList<ServerGameController> games = new List<ServerGameController>();
		private TcpClient? currentlyWaitingTcpClient = null;
		private Task<TcpClient>? currTcpClient;

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
			CardRepo = new ServerCardRepository();
		}

		public override void _Process(double delta)
		{
			_ = currTcpClient ?? throw new NotReadyYetException();
			_ = listener ?? throw new NotReadyYetException();
			_ = CardRepo ?? throw new NotReadyYetException();
			if (currTcpClient.IsCompleted)
			{
				var client = currTcpClient.Result;

				if (currentlyWaitingTcpClient == null) currentlyWaitingTcpClient = client;
				else
				{
					var gameController = GamePrefab.Instantiate() as ServerGameController
						?? throw new System.NotSupportedException("Server Game prefab wasn't a ServerGameController!");
					AddChild(gameController);
					gameController.Init(new TcpClient[] { currentlyWaitingTcpClient, client }, CardRepo, () => DebugMode);
					games.Add(gameController);
					currentlyWaitingTcpClient = null;
				}

				currTcpClient = listener.AcceptTcpClientAsync();
			}
		}
	}
}