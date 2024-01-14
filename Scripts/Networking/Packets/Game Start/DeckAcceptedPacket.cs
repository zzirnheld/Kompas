using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class DeckAcceptedPacket : Packet
	{
		public DeckAcceptedPacket() : base(DeckAccepted) { }

		public override Packet Copy() => new DeckAcceptedPacket();
	}
}

namespace Kompas.Client.Networking
{
	public class DeckAcceptedClientPacket : DeckAcceptedPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
			=> clientGame.ClientGameController.GameStartController.DeckAccepted();
	}
}