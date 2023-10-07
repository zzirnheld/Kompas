using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;

namespace Kompas.Networking.Packets
{
	public class ToggleDecliningTargetPacket : Packet
	{
		public bool enabled;

		public ToggleDecliningTargetPacket() : base(ToggleDecliningTarget) { }

		public ToggleDecliningTargetPacket(bool enabled) : this()
		{
			this.enabled = enabled;
		}

		public override Packet Copy() => new ToggleDecliningTargetPacket(enabled);
	}
}

namespace Kompas.Networking.Client
{
	public class ToggleDecliningTargetClientPacket : ToggleDecliningTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			if (enabled) clientGame.clientUIController.effectsUIController.EnableDecliningTarget();
			else clientGame.clientUIController.effectsUIController.DisableDecliningTarget();
		}
	}
}