using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Networking.Packets
{
	public class AnnihilateCardPacket : Packet
	{
		public int cardId;
		public string json;
		public int controllerIndex;

		public AnnihilateCardPacket() : base(AnnihilateCard) { }

		public AnnihilateCardPacket(int cardId, string json, int controllerIndex, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.json = json;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new AnnihilateCardPacket(cardId, json, controllerIndex, invert: false);

		public override Packet GetInversion(bool known)
		{
			if (known) return new AnnihilateCardPacket(cardId, json, controllerIndex, invert: true);
			else return new AddCardPacket(cardId, json, Location.Annihilation, controllerIndex, nowKnown: true, invert: true);
		}
	}
}

namespace Kompas.Client.Networking
{
	public class AnnihilateCardClientPacket : AnnihilateCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
			=> clientGame.LookupCardByID(cardId)?.Annihilate();
	}
}