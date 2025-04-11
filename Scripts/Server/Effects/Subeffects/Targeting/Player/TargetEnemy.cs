using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetEnemy : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var player = GetPlayerTarget(resolution.Context) ?? throw new NullPlayerException("No player to get the enemy of");
		resolution.Context.PlayerTargets.Add(player.Enemy);
		return Task.FromResult(ResolutionInfo.Next);
	}
}
