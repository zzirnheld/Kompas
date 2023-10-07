using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;
using Kompas.Cards.Movement;

namespace Kompas.Networking.Packets
{
	public class TopdeckCardPacket : Packet
	{
		public int cardId;
		public int controllerIndex;

		public TopdeckCardPacket() : base(TopdeckCard) { }

		public TopdeckCardPacket(int cardId, int controllerIndex, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new TopdeckCardPacket(cardId, controllerIndex);

		public override Packet GetInversion(bool known)
		{
			if (known) return new DeleteCardPacket(cardId);
			else return null;
		}
	}
}

namespace Kompas.Networking.Client
{
	public class TopdeckCardClientPacket : TopdeckCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var controller = clientGame.Players[controllerIndex];
			clientGame.LookupCardByID(cardId)?.Topdeck(controller);
		}
	}
}