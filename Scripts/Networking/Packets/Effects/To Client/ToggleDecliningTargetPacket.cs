using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

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

namespace Kompas.Client.Networking
{
	public class ToggleDecliningTargetClientPacket : ToggleDecliningTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.ClientGameController.TargetingController.CanDeclineFurtherTargets = enabled;
		}
	}
}