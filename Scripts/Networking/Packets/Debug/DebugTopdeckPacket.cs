using Kompas.Cards.Movement;
using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugTopdeckPacket : Packet
	{
		public int cardId;

		public DebugTopdeckPacket() : base(DebugTopdeck) { }

		public DebugTopdeckPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new DebugTopdeckPacket(cardId);
	}
}

namespace KompasServer.Networking
{
	public class DebugTopdeckServerPacket : DebugTopdeckPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			var card = serverGame.LookupCardByID(cardId);
			if (card == null)
				return Task.CompletedTask;
			else if (serverGame.UIController.DebugMode)
			{
				GD.PrintErr($"Debug topdecking card with id {cardId}");
				card.Topdeck();
			}
			else
			{
				GD.PrintErr($"Tried to debug topdeck card with id {cardId} while NOT in debug mode!");
				player.notifier.NotifyPutBack();
			}
			return Task.CompletedTask;
		}
	}
}