using Kompas.Networking.Packets;
using Kompas.Server.Effects;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class ActivateEffectActionPacket : Packet
	{
		public int cardId;
		public int effIndex;

		public ActivateEffectActionPacket() : base(ActivateEffectAction) { }

		public ActivateEffectActionPacket(int cardId, int effIndex) : this()
		{
			this.cardId = cardId;
			this.effIndex = effIndex;
		}

		public override Packet Copy() => new ActivateEffectActionPacket(cardId, effIndex);
	}
}

namespace Kompas.Server.Networking
{
	public class ActivateEffectActionServerPacket : ActivateEffectActionPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			var card = serverGame.LookupServerCardByID(cardId);
			if (card == null) return;
			var eff = card.ServerEffects[effIndex];
			if (eff == null) return;

			await player.TryActivateEffect(eff);
		}
	}
}