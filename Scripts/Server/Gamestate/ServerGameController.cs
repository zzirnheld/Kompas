using Godot;
using Kompas.Gamestate;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;
using Kompas.Shared.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Kompas.Server.Gamestate
{
	public partial class ServerGameController : GameController
	{
		private ServerGame? _serverGame;
		public ServerGame ServerGame => _serverGame
			?? throw new NotInitializedException();
		public override IGame Game => ServerGame;

		private ServerCardRepository? _cardRepository;
		public ServerCardRepository CardRepository => _cardRepository
			?? throw new NotInitializedException();

		private IReadOnlyCollection<ServerNetworker>? _networkers;
		public IReadOnlyCollection<ServerNetworker> Networkers => _networkers
			?? throw new NotInitializedException();

		public void Init(TcpClient[] tcpClients, ServerCardRepository cardRepository, System.Func<bool> debugMode)
		{
			_cardRepository = cardRepository;
			_serverGame = ServerGame.Create(this, CardRepository, debugMode);
			
			var players = ServerPlayer.Create(this,
				(player, index) => new ServerNetworker(tcpClients[index], player, ServerGame));
			_networkers = players.Select(p => p.Networker).ToArray();
			ServerGame.SetPlayers(players);
		}

		//Remember, async voids don't get awaited.
		//This means that Process will get called again before this call completes,
		//if and only if networker.Tick returns an incomplete Task (i.e. calls something else)
		public override async void _Process(double delta)
		{
			base._Process(delta);

			if (Networkers == null) return;
			foreach (var networker in Networkers) await networker.Tick();
		}
	}
}