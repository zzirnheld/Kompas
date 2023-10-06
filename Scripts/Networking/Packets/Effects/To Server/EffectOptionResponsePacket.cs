using Kompas.Networking.Packets;
using KompasServer.GameCore;
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

namespace KompasServer.Networking
{
	public class EffectOptionResponseServerPacket : EffectOptionResponsePacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			awaiter.EffOption = option;
			return Task.CompletedTask;
		}
	}
}