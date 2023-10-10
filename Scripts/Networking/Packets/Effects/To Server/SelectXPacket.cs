using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
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

namespace Kompas.Server.Networking
{
	public class SelectXServerPacket : SelectXPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			awaiter.PlayerXChoice = x;
			return Task.CompletedTask;
		}
	}
}