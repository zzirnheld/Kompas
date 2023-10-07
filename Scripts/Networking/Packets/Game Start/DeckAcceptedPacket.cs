using Kompas.Gamestate.Client;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class DeckAcceptedPacket : Packet
	{
		public DeckAcceptedPacket() : base(DeckAccepted) { }

		public override Packet Copy() => new DeckAcceptedPacket();
	}
}

namespace Kompas.Networking.Client
{
	public class DeckAcceptedClientPacket : DeckAcceptedPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
			=> clientGame.clientUIController.connectionUIController.DeckAccepted();
	}
}