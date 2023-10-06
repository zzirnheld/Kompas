using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Players;
using Kompas.Client.Networking;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class AddCardPacket : Packet
	{
		public int cardId;
		public string json;
		public int location;
		public int controllerIndex;
		public int x;
		public int y;
		public bool attached;
		public bool known;

		protected Location Location => (Location)location;

		public AddCardPacket() : base(AddCard) { }

		public AddCardPacket(int cardId, string json, Location location, int controllerIndex, bool nowKnown = false, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.json = json;
			this.location = (int)location;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
			this.known = nowKnown;
		}

		public AddCardPacket(int cardId, string json, Location location, int controllerIndex,
			int x, int y, bool attached, bool known, bool invert = false)
			: this(cardId, json, location, controllerIndex, invert: invert)
		{
			this.x = invert ? 6 - x : x;
			this.y = invert ? 6 - y : y;
			this.attached = attached;
			this.known = known;
		}

		//TODO allow for card to be added with stats not as defaults.
		//this will require using a json library that allows for polymorphism-ish stuff
		public AddCardPacket(GameCard card, bool invert = false)
			: this(card, card.KnownToEnemy, invert)
		{ }

		public AddCardPacket(GameCard card, bool known, bool invert = false)
			: this(cardId: card.ID, json: card.BaseJson, location: card.Location, controllerIndex: card.ControllerIndex,
				  x: card.Position?.x ?? 0, y: card.Position?.y ?? 0, attached: card.Attached, known: known, invert: invert)
		{ }

		public override Packet Copy() => new AddCardPacket(cardId, json, Location, controllerIndex, x, y, attached, known);

		public override Packet GetInversion(bool known)
		{
			if (IGame.IsHiddenLocation(Location))
			{
				return Location switch
				{
					Location.Hand => new ChangeEnemyHandCountPacket(1),
					Location.Deck => null,
					_ => throw new System.ArgumentException($"What should add card packet do when a card is added to the hidden location {location}"),
				};
			}
			else return new AddCardPacket(cardId, json, Location, controllerIndex, x, y, attached, known, invert: true);
		}
	}
}

namespace Kompas.Client.Networking
{
	public class AddCardClientPacket : AddCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var controller = clientGame.Players[controllerIndex];
			var card = clientGame.ClientCardRepository.InstantiateClientNonAvatar(json, controller, cardId, clientGame);
			card.KnownToEnemy = known;
			switch (Location)
			{
				case Location.Nowhere: break;
				case Location.Board:
					if (attached) clientGame.Board.GetCardAt((x, y)).AddAugment(card);
					else card.Play((x, y), controller);
					break;
				case Location.Discard:
					card.Discard();
					break;
				case Location.Hand:
					card.Rehand();
					break;
				case Location.Deck:
					card.Topdeck();
					break;
				case Location.Annihilation:
					card.Annihilate();
					break;
				default:
					throw new System.ArgumentException($"Invalid location {location} for Add Card Client Packet to put card");
			}
		}
	}
}