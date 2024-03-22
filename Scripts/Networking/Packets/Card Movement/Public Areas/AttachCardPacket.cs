using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;

namespace Kompas.Networking.Packets
{
	public class AttachCardPacket : Packet
	{
		public int cardId;
		public string json = string.Empty;
		public int controllerIndex;
		public int x;
		public int y;

		public AttachCardPacket() : base(AttachCard) { }

		public AttachCardPacket(int cardId, string json, int controllerIndex, int x, int y, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.json = json;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
			this.x = invert ? 6 - x : x;
			this.y = invert ? 6 - y : y;
		}

		public AttachCardPacket(IGameCard card, int x, int y, bool invert = false)
			: this(card.ID, card.BaseJson, card.ControllingPlayer.Index, x, y, invert: invert)
		{ }

		public override Packet Copy() => new AttachCardPacket(cardId, json, controllerIndex, x, y);

		public override Packet? GetInversion(bool known)
		{
			if (known) return new AttachCardPacket(cardId, json, 1 - controllerIndex, x, y, invert: true);
			else return new AddCardPacket(cardId, json, Location.Board, controllerIndex, x, y, attached: true, known: true, invert: true);
		}
	}
}

namespace Kompas.Client.Networking
{
	public class AttachCardClientPacket : AttachCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var toAttach = clientGame.LookupCardByID(cardId);
			var attachedTo = clientGame.Board.GetCardAt((x, y));

			if (toAttach != null) attachedTo?.AddAugment(toAttach);
		}
	}
}