using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class AttackActionPacket : Packet
	{
		public int attackerId;
		public int defenderId;

		public AttackActionPacket() : base(AttackAction) { }

		public AttackActionPacket(int attackerId, int defenderId) : this()
		{
			this.attackerId = attackerId;
			this.defenderId = defenderId;
		}

		public override Packet Copy() => new AttackActionPacket(attackerId, defenderId);
	}
}

namespace KompasServer.Networking
{
	public class AttackActionServerPacket : AttackActionPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			var attacker = serverGame.LookupCardByID(attackerId);
			var defender = serverGame.LookupCardByID(defenderId);
			await player.TryAttack(attacker, defender);
		}
	}
}