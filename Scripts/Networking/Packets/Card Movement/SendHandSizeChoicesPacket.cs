using Godot;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class SendHandSizeChoicesPacket : Packet
	{
		public int[] cardIds;

		public SendHandSizeChoicesPacket() : base(HandSizeChoices) { }

		public SendHandSizeChoicesPacket(int[] cardIds) : this()
		{
			GD.Print($"Hand size choices {string.Join(", ", cardIds)}");
			this.cardIds = cardIds;
		}

		public override Packet Copy() => new SendHandSizeChoicesPacket(cardIds);
	}
}

namespace Kompas.Server.Networking
{
	public class SendHandSizeChoicesServerPacket : SendHandSizeChoicesPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			awaiter.HandSizeChoices = cardIds;
			return Task.CompletedTask;
		}
	}
}