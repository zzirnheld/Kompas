using Kompas.Effects.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Gamestate;
using Kompas.Server.Networking;

namespace Kompas.Server.Effects.Models;

public class ServerHandSizeStackable : HandSizeStackable, IServerStackable
{
	public ServerHandSizeStackable(ServerGame serverGame, IPlayer controller)
		: base(serverGame, controller)
	{
		//tell the players this is here now
		ServerNotifier.NotifyHandSizeToStack(controller); //TODO move to a Declare method
	}
}