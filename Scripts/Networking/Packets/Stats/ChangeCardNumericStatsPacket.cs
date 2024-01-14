using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Kompas.Cards.Models;

namespace Kompas.Networking.Packets
{
	public class ChangeCardNumericStatsPacket : Packet
	{
		public int cardId;
		public int n;
		public int e;
		public int s;
		public int w;
		public int c;
		public int a;
		public int spacesMoved;

		public ChangeCardNumericStatsPacket() : base(UpdateCardNumericStats) { }

		public ChangeCardNumericStatsPacket(int cardId, CardStats stats, int spacesMoved) : this()
		{
			this.cardId = cardId;
			(n, e, s, w, c, a) = stats;
			this.spacesMoved = spacesMoved;
		}

		public override Packet Copy() => new ChangeCardNumericStatsPacket(cardId, (n, e, s, w, c, a), spacesMoved);
	}
}

namespace Kompas.Client.Networking
{
	public class ChangeCardNumericStatsClientPacket : ChangeCardNumericStatsPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardId);
			if (card != null)
			{
				card.SetStats((n, e, s, w, c, a));
				card.SpacesMoved = spacesMoved;
			}
		}
	}
}
