using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;

namespace Kompas.Networking.Packets
{
	public class SpacesMovedPacket : Packet
	{
		public int cardId;
		public int spacesMoved;

		public SpacesMovedPacket() : base(SpacesMoved) { }

		public SpacesMovedPacket(int cardId, int spacesMoved) : this()
		{
			this.cardId = cardId;
			this.spacesMoved = spacesMoved;
		}

		public override Packet Copy() => new SpacesMovedPacket(cardId, spacesMoved);
	}
}

namespace Kompas.Networking.Client
{
	public class SpacesMovedClientPacket : SpacesMovedPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardId);
			if (card != null)
			{
				card.SpacesMoved = spacesMoved;
				clientGame.UIController.CardViewController.Refresh();
			}
		}
	}
}