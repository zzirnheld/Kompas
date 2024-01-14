using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class SetDeckCountPacket : Packet
	{
		public int playerIndex;
		public int count;

		public SetDeckCountPacket() : base(SetDeckCount) { }

		public SetDeckCountPacket(int playerIndex, int count, bool invert = false) : this()
		{
			this.playerIndex = invert ? 1 - playerIndex : playerIndex;
			this.count = count;
		}

		public override Packet Copy() => new SetDeckCountPacket(playerIndex, count, invert: false);

		public override Packet? GetInversion(bool known) => new SetDeckCountPacket(playerIndex, count, invert: true);
	}
}

namespace Kompas.Client.Networking
{
	public class SetDeckCountClientPacket : SetDeckCountPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			//throw new System.NotImplementedException();
			//clientGame.clientPlayers[playerIndex].deckController.DeckCount = count;
		}
	}
}
