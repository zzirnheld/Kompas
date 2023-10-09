using Godot;
using Kompas.Cards.Movement;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugDiscardPacket : Packet
	{
		public int cardId;

		public DebugDiscardPacket() : base(DebugDiscard) { }

		public DebugDiscardPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new DebugDiscardPacket(cardId);
	}
}

namespace Kompas.Server.Networking
{
	public class DebugDiscardServerPacket : DebugDiscardPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			var card = serverGame.LookupCardByID(cardId);
			if (card == null)
				return Task.CompletedTask;
			else if (serverGame.DebugMode)
			{
				GD.PrintErr($"Debug discarding card with id {cardId}");
				card.Discard();
			}
			else
			{
				GD.PrintErr($"Tried to debug discard card with id {cardId} while NOT in debug mode!");
				Notifier.NotifyPutBack();
			}
			return Task.CompletedTask;
		}
	}
}