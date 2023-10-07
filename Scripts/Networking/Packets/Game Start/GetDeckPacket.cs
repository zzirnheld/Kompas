using Kompas.Gamestate.Client;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class GetDeckPacket : Packet
	{
		public GetDeckPacket() : base(GetDeck) { }

		public override Packet Copy() => new GetDeckPacket();
	}
}

namespace Kompas.Networking.Client
{
	public class GetDeckClientPacket : GetDeckPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
			=> clientGame.clientUIController.connectionUIController.Show(UI.ConnectionUIController.ConnectionState.SelectDeck);
	}
}