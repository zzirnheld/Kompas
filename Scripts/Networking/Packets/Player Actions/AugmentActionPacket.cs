using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class AugmentActionPacket : Packet
	{
		public int cardId;
		public int x;
		public int y;

		public AugmentActionPacket() : base(AugmentAction) { }

		public AugmentActionPacket(int cardId, int x, int y) : this()
		{
			this.cardId = cardId;
			this.x = x;
			this.y = y;
		}

		public override Packet Copy() => new AugmentActionPacket(cardId, x, y);
	}
}

namespace Kompas.Server.Networking
{
	public class AugmentActionServerPacket : AugmentActionPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			if (player.Index == 1)
			{
				x = 6 - x;
				y = 6 - y;
			}
			var card = serverGame.LookupCardByID(cardId);
			await player.TryAugment(card, (x, y));
		}
	}
}