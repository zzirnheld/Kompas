using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class SpaceTargetPacket : Packet
	{
		public int x;
		public int y;

		public SpaceTargetPacket() : base(SpaceTargetChosen) { }

		public SpaceTargetPacket(int x, int y) : this()
		{
			this.x = x;
			this.y = y;
		}

		public override Packet Copy() => new SpaceTargetPacket(x, y);
	}
}

namespace Kompas.Server.Networking
{
	public class SpaceTargetServerPacket : SpaceTargetPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			if (player.Index != 0)
			{
				x = 6 - x;
				y = 6 - y;
			}

			serverGame.Awaiter.SpaceTarget = (x, y);
			return Task.CompletedTask;
		}
	}
}