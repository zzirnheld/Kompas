using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class SetTurnPlayerPacket : Packet
	{
		public int turnPlayer;

		public SetTurnPlayerPacket() : base(SetTurnPlayer) { }

		public SetTurnPlayerPacket(int turnPlayer, bool invert = false) : this()
		{
			this.turnPlayer = invert ? 1 - turnPlayer : turnPlayer;
		}

		public override Packet Copy() => new SetTurnPlayerPacket(turnPlayer);

		public override Packet GetInversion(bool known = true) => new SetTurnPlayerPacket(turnPlayer, invert: true);
	}
}

namespace Kompas.Client.Networking
{
	public class SetTurnPlayerClientPacket : SetTurnPlayerPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.SetTurn(turnPlayer);
	}
}