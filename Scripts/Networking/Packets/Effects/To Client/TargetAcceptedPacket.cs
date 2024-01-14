using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class TargetAcceptedPacket : Packet
	{
		public TargetAcceptedPacket() : base(TargetAccepted) { }

		public override Packet Copy() => new TargetAcceptedPacket();
	}
}

namespace Kompas.Client.Networking
{
	public class TargetAcceptedClientPacket : TargetAcceptedPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.ClientGameController.TargetingController.TargetAccepted();
		}
	}
}