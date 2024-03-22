using Kompas.Cards.Models;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kompas.Server.Gamestate.Players;
using Godot;
using Kompas.Shared.Enumerable;

namespace Kompas.Networking.Packets
{
	public class ListChoicesPacket : Packet
	{
		public int[]? cardIds;

		public ListChoicesPacket() : base(ListChoicesChosen) { }

		public ListChoicesPacket(int[] cardIds) : this()
		{
			this.cardIds = cardIds;
		}

		public ListChoicesPacket(IEnumerable<IGameCard> cards) : this(cards.Select(c => c.ID).ToArray()) { }

		public override Packet Copy() => new ListChoicesPacket()
		{
			cardIds = cardIds
		};
	}
}

namespace Kompas.Server.Networking
{
	public class ListChoicesServerPacket : ListChoicesPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			if (cardIds == null)
			{
				GD.PushWarning("Null card ids for choices");
				return Task.CompletedTask;
			}
			var choices = cardIds.Select(c => serverGame.LookupCardByID(c)).NonNull().Distinct();
			serverGame.Awaiter.CardListTargets = choices;
			return Task.CompletedTask;
		}
	}
}