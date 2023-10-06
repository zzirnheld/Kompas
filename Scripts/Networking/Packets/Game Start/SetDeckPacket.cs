using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class SetDeckPacket : Packet
	{
		public string decklist = "";

		public SetDeckPacket() : base(SetDeck) { }

		public SetDeckPacket(string decklist) : this()
		{
			this.decklist = decklist;
		}

		public override Packet Copy() => new SetDeckPacket(decklist);
	}
}

namespace KompasServer.Networking
{
	public class SetDeckServerPacket : SetDeckPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			await serverGame.SetDeck(player, decklist);
		}
	}
}