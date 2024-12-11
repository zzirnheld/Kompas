using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Movement;
using Kompas.Gamestate;

namespace Kompas.Networking.Packets
{
	public class MoveCardPacket : Packet
	{
		public int cardId;
		public int x;
		public int y;

		public MoveCardPacket() : base(MoveCard) { }

		public MoveCardPacket(int cardId, int x, int y, bool invert) : this()
		{
			this.cardId = cardId;
			this.x = invert ? Space.MaxIndex - x : x;
			this.y = invert ? Space.MaxIndex - y : y;
		}

		public override Packet Copy() => new MoveCardPacket(cardId, x, y, invert: false);

		public override Packet? GetInversion(bool known) => new MoveCardPacket(cardId, x, y, invert: true);
	}
}

namespace Kompas.Client.Networking
{
	public class MoveCardClientPacket : MoveCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.LookupCardByID(cardId)?.Move((x, y), normalMove: false, mover: null);
			//TODO have move in client call refresh. for that matter, position change
		}
	}
}