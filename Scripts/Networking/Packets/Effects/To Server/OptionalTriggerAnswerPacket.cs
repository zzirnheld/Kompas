using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class OptionalTriggerAnswerPacket : Packet
	{
		public bool answer;

		public OptionalTriggerAnswerPacket() : base(OptionalTriggerResponse) { }

		public OptionalTriggerAnswerPacket(bool answer) : this()
		{
			this.answer = answer;
		}

		public override Packet Copy() => new OptionalTriggerAnswerPacket(answer);
	}
}

namespace Kompas.Server.Networking
{
	public class OptionalTriggerAnswerServerPacket : OptionalTriggerAnswerPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			awaiter.OptionalTriggerAnswer = answer;
			return Task.CompletedTask;
		}
	}
}