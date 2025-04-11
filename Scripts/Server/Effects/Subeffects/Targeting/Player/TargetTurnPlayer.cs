using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTurnPlayer : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.PlayerTargets.Add(Game.TurnPlayer);
		return Task.FromResult(ResolutionInfo.Next);
	}
}
