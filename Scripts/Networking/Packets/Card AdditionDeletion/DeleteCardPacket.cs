using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class DeleteCardPacket : Packet
	{
		public int cardId;

		public DeleteCardPacket() : base(DeleteCard) { }

		public DeleteCardPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new DeleteCardPacket(cardId);
	}
}

namespace Kompas.Client.Networking
{
	public class DeleteCardClientPacket : DeleteCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardId);
			clientGame.Delete(card);
		}
	}
}