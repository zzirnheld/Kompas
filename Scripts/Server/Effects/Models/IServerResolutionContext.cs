using Kompas.Effects.Models;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Effects.Models
{
	public interface IServerResolutionContext : IResolutionContext
	{
		public ServerPlayer ControllingPlayer { get; }
	}
}