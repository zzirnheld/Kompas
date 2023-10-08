using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class PassPriorityPacket : Packet
	{
		public PassPriorityPacket() : base(PassPriority) { }

		public override Packet Copy() => new PassPriorityPacket();
	}
}

namespace Kompas.Server.Networking
{
	public class PassPriorityServerPacket : PassPriorityPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			throw new System.NotImplementedException();
			//player.PassedPriority = true;
			//await serverGame.EffectsController.CheckForResponse(reset: false);
		}
	}
}