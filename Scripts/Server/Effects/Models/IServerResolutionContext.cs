using Kompas.Effects.Models;
using Kompas.Server.Effects.Models.Subeffects;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Effects.Models;

public interface IServerResolutionContext : IResolutionContext
{
	public ServerPlayer ControllingPlayer { get; }
	public IServerSubeffect? OnImpossible { get; set; }
}