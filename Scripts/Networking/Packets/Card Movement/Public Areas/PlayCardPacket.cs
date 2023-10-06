using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Networking.Packets
{
	public class PlayCardPacket : Packet
	{
		public int cardId;
		public string json;
		public int controllerIndex;
		public int x;
		public int y;

		public PlayCardPacket() : base(PlayCard) { }

		public PlayCardPacket(int cardId, string json, int controllerIndex, int x, int y, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.json = json;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
			this.x = invert ? 6 - x : x;
			this.y = invert ? 6 - y : y;
		}

		public override Packet Copy() => new PlayCardPacket(cardId, json, controllerIndex, x, y, invert: false);

		public override Packet GetInversion(bool known)
		{
			if (known) return new PlayCardPacket(cardId, json, controllerIndex, x, y, invert: true);
			else return new AddCardPacket(cardId, json, Location.Board, controllerIndex, x, y, attached: false, known: true, invert: true);
		}
	}
}

namespace Kompas.Client.Networking
{
	public class PlayCardClientPacket : PlayCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var controller = clientGame.Players[controllerIndex];
			var card = clientGame.LookupCardByID(cardId);
			card.Play((x, y), controller);
		}
	}
}