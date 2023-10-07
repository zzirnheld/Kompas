using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;
using Kompas.Cards.Movement;

namespace Kompas.Networking.Packets
{
	public class BottomdeckCardPacket : Packet
	{
		public int cardId;
		public int controllerIndex;

		public BottomdeckCardPacket() : base(BottomdeckCard) { }

		public BottomdeckCardPacket(int cardId, int controllerIndex, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new BottomdeckCardPacket(cardId, controllerIndex);

		public override Packet GetInversion(bool known)
		{
			if (known) return new DeleteCardPacket(cardId);
			else return null;
		}
	}
}

namespace Kompas.Networking.Client
{
	public class BottomdeckCardClientPacket : BottomdeckCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var controller = clientGame.Players[controllerIndex];
			var card = clientGame.LookupCardByID(cardId);
			card?.Bottomdeck(controller);
		}
	}
}