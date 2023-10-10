using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Shared;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class SetDeckPacket : Packet
	{
		public Decklist decklist;

		public SetDeckPacket() : base(SetDeck) { }

		public SetDeckPacket(Decklist decklist) : this()
		{
			this.decklist = decklist;
		}

		public override Packet Copy() => new SetDeckPacket(decklist);
	}
}

namespace Kompas.Server.Networking
{
	public class SetDeckServerPacket : SetDeckPacket, IServerOrderPacket
	{
		public async Task Execute(ServerGame serverGame, ServerPlayer player)
		{
			await serverGame.SetDeck(player, decklist);
		}
	}
}