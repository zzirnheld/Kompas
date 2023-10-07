using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;

namespace Kompas.Networking.Packets
{
	public class PutCardsBackPacket : Packet
	{
		public PutCardsBackPacket() : base(PutCardsBack) { }

		public override Packet Copy() => new PutCardsBackPacket();
	}
}

namespace Kompas.Networking.Client
{
	public class PutCardsBackClientPacket : PutCardsBackPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => throw new System.NotImplementedException(); //Maybe this isn't necessary anymore?
	}
}