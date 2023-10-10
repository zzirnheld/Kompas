﻿using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class PlayActionPacket : Packet
	{
		public int cardId;
		public int x;
		public int y;

		public PlayActionPacket() : base(PlayAction) { }

		public PlayActionPacket(int cardId, int x, int y) : this()
		{
			this.cardId = cardId;
			this.x = x;
			this.y = y;
		}

		public override Packet Copy() => new PlayActionPacket(cardId, x, y);
	}
}

namespace Kompas.Server.Networking
{
	public class PlayActionServerPacket : PlayActionPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			if (player.Index == 1)
			{
				x = 6 - x;
				y = 6 - y;
			}
			var card = serverGame.LookupCardByID(cardId);
			await player.TryPlay(card, (x, y));
		}
	}
}