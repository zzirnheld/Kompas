using Godot;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
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

namespace Kompas.Server.Networking
{
	public class DebugSetPipsServerPacket : DebugSetPipsPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{

			throw new System.NotImplementedException();
			/*
			if (serverGame.DebugMode)
			{
				GD.PrintErr($"Debug setting player {Player.Index} pips to {numPips}");
				player.Pips = numPips;
			}
			else GD.PrintErr($"Tried to debug set pips of player {Player.Index} to {numPips} while NOT in debug mode!");
			return Task.CompletedTask;
			*/
		}
	}
}