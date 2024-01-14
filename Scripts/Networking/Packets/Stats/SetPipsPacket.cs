using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class SetPipsPacket : Packet
	{
		public int numPips;
		public int controllerIndex;

		public SetPipsPacket() : base(SetPips) { }

		public SetPipsPacket(int cardId, int controllerIndex, bool invert = false) : this()
		{
			this.numPips = cardId;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new SetPipsPacket(numPips, controllerIndex);

		public override Packet? GetInversion(bool known) => new SetPipsPacket(numPips, controllerIndex, invert: true);
	}
}

namespace Kompas.Client.Networking
{
	public class SetPipsClientPacket : SetPipsPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var controller = clientGame.Players[controllerIndex];
			if (controller != null) controller.Pips = numPips;
		}
	}
}