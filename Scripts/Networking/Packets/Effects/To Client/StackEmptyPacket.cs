using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class StackEmptyPacket : Packet
	{
		public StackEmptyPacket() : base(StackEmpty) { }

		public override Packet Copy() => new StackEmptyPacket();
	}
}

namespace Kompas.Client.Networking
{
	public class StackEmptyClientPacket : StackEmptyPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.StackEmptied();
	}
}