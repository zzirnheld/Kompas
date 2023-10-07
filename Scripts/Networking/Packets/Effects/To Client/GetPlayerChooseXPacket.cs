using Kompas.Gamestate.Client;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class GetPlayerChooseXPacket : Packet
	{
		public GetPlayerChooseXPacket() : base(PlayerChooseX) { }

		public override Packet Copy() => new GetPlayerChooseXPacket();
	}
}

namespace Kompas.Networking.Client
{
	public class GetPlayerChooseXClientPacket : GetPlayerChooseXPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.clientUIController.effectsUIController.GetXForEffect();
		}
	}
}