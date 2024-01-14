using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class NegateCardPacket : Packet
	{
		public int cardId;
		public bool negated;

		public NegateCardPacket() : base(NegateCard) { }

		public NegateCardPacket(int cardId, bool negated) : this()
		{
			this.cardId = cardId;
			this.negated = negated;
		}

		public override Packet Copy() => new NegateCardPacket(cardId, negated);
	}
}

namespace Kompas.Client.Networking
{
	public class NegateCardClientPacket : NegateCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardId);
			if (card != null) card.SetNegated(negated);
		}
	}
}