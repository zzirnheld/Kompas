using Godot;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugSetNESWPacket : Packet
	{
		public int cardId;
		public int n;
		public int e;
		public int s;
		public int w;

		public DebugSetNESWPacket() : base(DebugSetNESW) { }

		public DebugSetNESWPacket(int cardId, int n, int e, int s, int w) : this()
		{
			this.cardId = cardId;
			this.n = n;
			this.e = e;
			this.s = s;
			this.w = w;
		}

		public override Packet Copy() => new DebugSetNESWPacket(cardId, n, e, s, w);
	}
}

namespace Kompas.Server.Networking
{
	public class DebugSetNESWServerPacket : DebugSetNESWPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{

			throw new System.NotImplementedException();
			/*
			var card = serverGame.LookupCardByID(cardId);
			if (card == null)
				return Task.CompletedTask;
			else if (serverGame.DebugMode)
			{
				GD.PrintErr($"Debug setting NESW to {n}, {e}, {s}, {w} of card with id {cardId}");
				card.SetCharStats(n, e, s, w);
			}
			else
			{
				GD.PrintErr($"Tried to debug set NESW of card with id {cardId} while NOT in debug mode!");
				card.CardController.PutBack();
			}
			return Task.CompletedTask;
			*/
		}
	}
}