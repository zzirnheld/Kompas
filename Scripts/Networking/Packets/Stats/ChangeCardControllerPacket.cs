using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class ChangeCardControllerPacket : Packet
	{
		public int cardId;
		public int controllerIndex;

		public ChangeCardControllerPacket() : base(ChangeCardController) { }

		public ChangeCardControllerPacket(int cardId, int controllerIndex, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new ChangeCardControllerPacket(cardId, controllerIndex);

		//don't try and tell the client to change the controller of a card they don't know about
		public override Packet? GetInversion(bool known) => known ? new ChangeCardControllerPacket(cardId, controllerIndex, invert: true) : null;
	}
}

namespace Kompas.Client.Networking
{
	public class ChangeCardControllerClientPacket : ChangeCardControllerPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardId);
			var controller = clientGame.Players[controllerIndex];
			if (card != null && controller != null) card.ControllingPlayer = controller;
			//If this fails, it's probably because the card doesn't exist, because it's a card that hasn't been sent to the client.
			// else GD.Print($"Could not set card controller, card: {card}; controller: {controller}");
		}
	}
}