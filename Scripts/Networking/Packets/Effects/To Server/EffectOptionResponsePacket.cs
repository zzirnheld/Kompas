using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class EffectOptionResponsePacket : Packet
	{
		public int option;

		public EffectOptionResponsePacket() : base(ChooseEffectOption) { }

		public EffectOptionResponsePacket(int option) : this()
		{
			this.option = option;
		}

		public override Packet Copy() => new EffectOptionResponsePacket(option);
	}
}

namespace Kompas.Server.Networking
{
	public class EffectOptionResponseServerPacket : EffectOptionResponsePacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			awaiter.EffOption = option;
			return Task.CompletedTask;
		}
	}
}