using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class PassPriorityPacket : Packet
	{
		public PassPriorityPacket() : base(PassPriority) { }

		public override Packet Copy() => new PassPriorityPacket();
	}
}

namespace KompasServer.Networking
{
	public class PassPriorityServerPacket : PassPriorityPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			//player.PassedPriority = true;
			await serverGame.effectsController.CheckForResponse(reset: false);
		}
	}
}