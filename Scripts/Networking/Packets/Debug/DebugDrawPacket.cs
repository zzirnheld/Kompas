using Kompas.Networking.Packets;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugDrawPacket : Packet
	{
		public DebugDrawPacket() : base(DebugDraw) { }

		public override Packet Copy() => new DebugDrawPacket();
	}
}

namespace KompasServer.Networking
{
	public class DebugDrawServerPacket : DebugDrawPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			if (serverGame.UIController.DebugMode)
			{
				GD.PrintErr($"Debug drawing");
				serverGame.Draw(player);
			}
			else GD.PrintErr($"Tried to debug draw while NOT in debug mode!");
			return Task.CompletedTask;
		}
	}
}