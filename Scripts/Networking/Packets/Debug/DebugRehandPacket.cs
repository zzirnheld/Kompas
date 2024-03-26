using Godot;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
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

namespace Kompas.Server.Networking
{
	public class DebugRehandServerPacket : DebugRehandPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			var card = serverGame.LookupCardByID(cardId);
			if (card == null)
				return Task.CompletedTask;

			throw new System.NotImplementedException();
			/*
			else if (serverGame.DebugMode)
			{
				Logger.Err($"Debug rehanding card with id {cardId}");
				card.Rehand();
			}
			else
			{
				Logger.Err($"Tried to debug rehand card with id {cardId} while NOT in debug mode!");
				Notifier.NotifyPutBack();
			}
			return Task.CompletedTask;
			*/
		}
	}
}