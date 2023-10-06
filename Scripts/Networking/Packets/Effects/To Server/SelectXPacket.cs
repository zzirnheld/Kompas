using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class SelectXPacket : Packet
	{
		public int x;

		public SelectXPacket() : base(XSelectionChosen) { }

		public SelectXPacket(int x) : this()
		{
			this.x = x;
		}

		public override Packet Copy() => new SelectXPacket(x);
	}
}

namespace KompasServer.Networking
{
	public class SelectXServerPacket : SelectXPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			awaiter.PlayerXChoice = x;
			return Task.CompletedTask;
		}
	}
}