using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Client.Effects.Models;

namespace Kompas.Networking.Packets
{
	public class AttackStartedPacket : Packet
	{
		public int attackerId;
		public int defenderId;
		public int controllerIndex;

		public AttackStartedPacket() : base(AttackStarted) { }

		public AttackStartedPacket(int attackerId, int defenderId, int controllerIndex) : this()
		{
			this.attackerId = attackerId;
			this.defenderId = defenderId;
			this.controllerIndex = controllerIndex;
		}

		public override Packet Copy() => new AttackStartedPacket(attackerId, defenderId, controllerIndex);

		public override Packet? GetInversion(bool known = true) => new AttackStartedPacket(attackerId, defenderId, 1 - controllerIndex);
	}
}

namespace Kompas.Client.Networking
{
	public class AttackStartedClientPacket : AttackStartedPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var attacker = clientGame.LookupCardByID(attackerId);
			var defender = clientGame.LookupCardByID(defenderId);
			var controller = clientGame.Players[controllerIndex];
			if (attacker != null && defender != null)
			{
				clientGame.StackController.Attacked(new ClientAttack(controller, attacker, defender));
			}
		}
	}
}