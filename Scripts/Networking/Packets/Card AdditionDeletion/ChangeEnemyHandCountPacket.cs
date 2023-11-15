using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class ChangeEnemyHandCountPacket : Packet
	{
		public int num;

		public ChangeEnemyHandCountPacket() : base(ChangeEnemyHandCount) { }

		public ChangeEnemyHandCountPacket(int num) : this()
		{
			this.num = num;
		}

		public override Packet Copy() => new ChangeEnemyHandCountPacket(num);
	}
}

namespace Kompas.Client.Networking
{
	public class ChangeEnemyHandCountClientPacket : ChangeEnemyHandCountPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			//TODO replace this cast with newing up a dummy and adding it to hand, so we don't have to cast :)
			for (int i = 0; i < num; i++) (clientGame.Players[1].Hand as ClientHand)?.IncrementHand();
			for (int i = 0; i > num; i--) (clientGame.Players[1].Hand as ClientHand)?.DecrementHand();
		}
	}
}