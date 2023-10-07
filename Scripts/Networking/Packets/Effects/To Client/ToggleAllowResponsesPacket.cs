using Kompas.Gamestate.Client;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class ToggleAllowResponsesPacket : Packet
	{
		public bool enabled;

		public ToggleAllowResponsesPacket() : base(ToggleAllowResponses) { }

		public ToggleAllowResponsesPacket(bool enabled) : this()
		{
			this.enabled = enabled;
		}

		public override Packet Copy() => new ToggleAllowResponsesPacket(enabled);
	}
}

namespace Kompas.Networking.Client
{
	public class ToggleAllowResponsesClientPacket : ToggleAllowResponsesPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			if (enabled) clientGame.clientUIController.effectsUIController.GetResponse();
			else clientGame.clientUIController.effectsUIController.UngetResponse();
		}
	}
}