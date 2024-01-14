using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class SetFirstPlayerPacket : Packet
	{
		public int playerIndex;

		public SetFirstPlayerPacket() : base(SetFirstTurnPlayer) { }

		public SetFirstPlayerPacket(int playerIndex) : this()
		{
			this.playerIndex = playerIndex;
		}

		public override Packet Copy() => new SetFirstPlayerPacket();
	}
}

namespace Kompas.Client.Networking
{
	public class SetFirstPlayerClientPacket : SetFirstPlayerPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.SetFirstTurnPlayer(playerIndex);
	}
}