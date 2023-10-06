using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugSetPipsPacket : Packet
	{
		public int numPips;

		public DebugSetPipsPacket() : base(DebugSetPips) { }

		public DebugSetPipsPacket(int numPips) : this()
		{
			this.numPips = numPips;
		}

		public override Packet Copy() => new DebugSetPipsPacket(numPips);
	}
}

namespace KompasServer.Networking
{
	public class DebugSetPipsServerPacket : DebugSetPipsPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			if (serverGame.UIController.DebugMode)
			{
				GD.PrintErr($"Debug setting player {player.index} pips to {numPips}");
				player.Pips = numPips;
			}
			else GD.PrintErr($"Tried to debug set pips of player {player.index} to {numPips} while NOT in debug mode!");
			return Task.CompletedTask;
		}
	}
}