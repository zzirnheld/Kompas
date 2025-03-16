using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTurnPlayer : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.playerTargets.Add(Game.TurnPlayer);
		return Task.FromResult(ResolutionInfo.Next);
	}
}
