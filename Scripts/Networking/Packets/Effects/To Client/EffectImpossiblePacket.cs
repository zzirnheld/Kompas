using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class EffectImpossiblePacket : Packet
	{
		public EffectImpossiblePacket() : base(EffectImpossible) { }

		public override Packet Copy() => new EffectImpossiblePacket();
	}
}

namespace Kompas.Client.Networking
{
	public class EffectImpossibleClientPacket : EffectImpossiblePacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) 
		{
			clientGame.ClientGameController.CurrentStateController.ShowCurrentStateInfo("Effect impossible!");
		} 
	}
}