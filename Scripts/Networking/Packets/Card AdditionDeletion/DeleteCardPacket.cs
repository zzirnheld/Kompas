using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;

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

namespace Kompas.Networking.Client
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