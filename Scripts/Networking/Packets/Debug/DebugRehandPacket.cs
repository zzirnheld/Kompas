using Kompas.Cards.Movement;
using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugRehandPacket : Packet
	{
		public int cardId;

		public DebugRehandPacket() : base(DebugRehand) { }

		public DebugRehandPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new DebugRehandPacket(cardId);
	}
}

namespace KompasServer.Networking
{
	public class DebugRehandServerPacket : DebugRehandPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			var card = serverGame.LookupCardByID(cardId);
			if (card == null)
				return Task.CompletedTask;
			else if (serverGame.UIController.DebugMode)
			{
				GD.PrintErr($"Debug rehanding card with id {cardId}");
				card.Rehand();
			}
			else
			{
				GD.PrintErr($"Tried to debug rehand card with id {cardId} while NOT in debug mode!");
				player.notifier.NotifyPutBack();
			}
			return Task.CompletedTask;
		}
	}
}