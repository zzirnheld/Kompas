using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class EndTurnActionPacket : Packet
	{
		public EndTurnActionPacket() : base(EndTurnAction) { }

		public override Packet Copy() => new EndTurnActionPacket();
	}
}

namespace KompasServer.Networking
{
	public class EndTurnActionServerPacket : EndTurnActionPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			await player.TryEndTurn();
		}
	}
}