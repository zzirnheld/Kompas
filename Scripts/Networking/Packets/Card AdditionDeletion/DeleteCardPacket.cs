using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Godot;

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
			if (card == null)
			{
				Logger.Err($"Can't delete card {cardId}");
				return;
			}
			clientGame.Delete(card);
		}
	}
}