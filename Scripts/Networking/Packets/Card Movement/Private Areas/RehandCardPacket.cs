using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Movement;
using Godot;

namespace Kompas.Networking.Packets
{
	public class RehandCardPacket : Packet
	{
		public int cardId;

		public RehandCardPacket() : base(RehandCard) { }

		public RehandCardPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new RehandCardPacket(cardId);

		public override Packet? GetInversion(bool known)
		{
			if (known) return new RehandCardPacket(cardId);
			else return new ChangeEnemyHandCountPacket(1);
		}
	}
}

namespace Kompas.Client.Networking
{
	public class RehandCardClientPacket : RehandCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			GD.Print($"Trying to hand {cardId} {clientGame.LookupCardByID(cardId)}");
			clientGame.LookupCardByID(cardId)?.Rehand();
		}
	}
}