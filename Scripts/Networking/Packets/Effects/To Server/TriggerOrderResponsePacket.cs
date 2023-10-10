﻿using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class TriggerOrderResponsePacket : Packet
	{
		public int[] cardIds;
		public int[] effIndices;
		public int[] orders;

		public TriggerOrderResponsePacket() : base(ChooseTriggerOrder) { }

		public TriggerOrderResponsePacket(int[] cardIds, int[] effIndices, int[] orders) : this()
		{
			this.cardIds = cardIds;
			this.effIndices = effIndices;
			this.orders = orders;
		}

		public override Packet Copy() => new TriggerOrderResponsePacket(cardIds, effIndices, orders);
	}
}

namespace Kompas.Server.Networking
{
	public class TriggerOrderResponseServerPacket : TriggerOrderResponsePacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			awaiter.TriggerOrders = (cardIds, effIndices, orders);
			return Task.CompletedTask;
		}
	}
}