using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class ActivateCardPacket : Packet
	{
		public int cardId;
		public bool activated;

		public ActivateCardPacket() : base(ActivateCard) { }

		public ActivateCardPacket(int cardId, bool activated) : this()
		{
			this.cardId = cardId;
			this.activated = activated;
		}

		public override Packet Copy() => new ActivateCardPacket(cardId, activated);
	}
}

namespace Kompas.Client.Networking
{
	public class ActivateCardClientPacket : ActivateCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardId);
			if (card != null) card.SetActivated(activated);
		}
	}
}