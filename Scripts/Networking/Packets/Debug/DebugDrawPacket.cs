using Godot;
using Kompas.Networking.Packets;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using System.Threading.Tasks;

namespace Kompas.Networking.Packets
{
	public class DebugDrawPacket : Packet
	{
		public DebugDrawPacket() : base(DebugDraw) { }

		public override Packet Copy() => new DebugDrawPacket();
	}
}

namespace Kompas.Server.Networking
{
	public class DebugDrawServerPacket : DebugDrawPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player)
		{

			throw new System.NotImplementedException();
			/*
			if (serverGame.DebugMode)
			{
				Logger.Err($"Debug drawing");
				serverGame.Draw(player);
			}
			else Logger.Err($"Tried to debug draw while NOT in debug mode!");
			return Task.CompletedTask;
			*/
		}
	}
}