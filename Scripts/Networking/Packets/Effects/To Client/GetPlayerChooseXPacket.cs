using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class GetPlayerChooseXPacket : Packet
	{
		public GetPlayerChooseXPacket() : base(PlayerChooseX) { }

		public override Packet Copy() => new GetPlayerChooseXPacket();
	}
}

namespace Kompas.Client.Networking
{
	public class GetPlayerChooseXClientPacket : GetPlayerChooseXPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.clientUIController.effectsUIController.GetXForEffect();
		}
	}
}