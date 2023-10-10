using Godot;
using Kompas.Gamestate;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Kompas.Server.Gamestate
{
	public partial class ServerGameController : GameController
	{
		public ServerGame ServerGame { get; private set; }
		public override IGame Game => ServerGame;

		public ServerCardRepository CardRepository = new();

		public ServerAwaiter Awaiter { get; private set; }
		public IReadOnlyCollection<ServerNetworker> Networkers { get; private set; }

		public void Init(TcpClient[] tcpClients)
		{
			ServerGame = ServerGame.Create(this, CardRepository);
			
			var players = ServerPlayer.Create(this,
				(player, index) => new ServerNetworker(tcpClients[index], player, ServerGame));
			Networkers = players.Select(p => p.Networker).ToArray();
			ServerGame.SetPlayers(players);
		}

		public override void _Process(double delta)
		{
			base._Process(delta);

			if (Networkers == null) return;
			foreach (var networker in Networkers) networker.Tick();
		}
	}
}