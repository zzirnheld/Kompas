using Godot;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class TriggerOrderResponsePacket : Packet
	{
		public int[]? cardIds;
		public int[]? effIndices;
		public int[]? orders;

		public TriggerOrderResponsePacket() : base(ChooseTriggerOrder) { }

		public TriggerOrderResponsePacket(int[] cardIds, int[] effIndices, int[] orders) : this()
		{
			this.cardIds = cardIds;
			this.effIndices = effIndices;
			this.orders = orders;
		}

		public override Packet Copy() => new TriggerOrderResponsePacket()
		{
			cardIds = cardIds,
			effIndices = effIndices,
			orders = orders
		};
	}
}

namespace Kompas.Server.Networking
{
	public class TriggerOrderResponseServerPacket : TriggerOrderResponsePacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			if (cardIds == null || effIndices == null || orders == null)
			{
				Logger.Warn("Null trigger order");
				return Task.CompletedTask;
			}
			serverGame.Awaiter.TriggerOrders = (cardIds, effIndices, orders);
			return Task.CompletedTask;
		}
	}
}