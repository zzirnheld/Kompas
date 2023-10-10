using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class CardTargetPacket : Packet
	{
		public int cardId;

		public CardTargetPacket() : base(CardTargetChosen) { }

		public CardTargetPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new CardTargetPacket(cardId);
	}
}

namespace Kompas.Server.Networking
{
	public class CardTargetServerPacket : CardTargetPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			awaiter.CardTarget = serverGame.LookupCardByID(cardId);
			return Task.CompletedTask;
		}
	}
}