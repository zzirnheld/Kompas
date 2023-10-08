using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DeclineAnotherTargetPacket : Packet
	{
		public DeclineAnotherTargetPacket() : base(DeclineAnotherTarget) { }

		public override Packet Copy() => new DeclineAnotherTargetPacket();
	}
}

namespace Kompas.Server.Networking
{
	public class DeclineAnotherTargetServerPacket : DeclineAnotherTargetPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			awaiter.DeclineTarget = true;
			return Task.CompletedTask;
		}
	}
}