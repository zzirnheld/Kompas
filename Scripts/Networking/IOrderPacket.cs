using System.Threading.Tasks;
using Kompas.Client.Gamestate;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;

//TODO: move these to the relevant places. figure out where to put them in the folder structure - for convenience, they should probably stay in their own area,
//since the server and client packets' neutral versions need to be visible to the other
namespace Kompas.Client.Networking
{
	public interface IClientOrderPacket
	{
		/// <summary>
		/// Executes the packet for the given client game.
		/// </summary>
		/// <param name="clientGame">The client game to execute the packet from.</param>
		void Execute(ClientGame clientGame); //TODO consider passing in the ClientGameController
	}
}

namespace Kompas.Server.Networking
{
	public interface IServerOrderPacket
	{
		/// <summary>
		/// Executes the packet for the given server game, 
		/// and the given player who it came from
		/// </summary>
		/// <param name="serverGame">The server game to apply the packet to.</param>
		/// <param name="player">The player who this packet came from.</param>
		//TODO server
		Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter);
	}
}