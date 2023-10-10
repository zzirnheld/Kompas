using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class EndTurnActionPacket : Packet
	{
		public EndTurnActionPacket() : base(EndTurnAction) { }

		public override Packet Copy() => new EndTurnActionPacket();
	}
}

namespace Kompas.Server.Networking
{
	public class EndTurnActionServerPacket : EndTurnActionPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			await player.TryEndTurn();
		}
	}
}