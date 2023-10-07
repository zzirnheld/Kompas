using Kompas.Gamestate.Client;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class EffectImpossiblePacket : Packet
	{
		public EffectImpossiblePacket() : base(EffectImpossible) { }

		public override Packet Copy() => new EffectImpossiblePacket();
	}
}

namespace Kompas.Networking.Client
{
	public class EffectImpossibleClientPacket : EffectImpossiblePacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.clientUIController.currentStateUIController.EffectImpossible();
	}
}