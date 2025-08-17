using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTurnPlayerData : SubeffectData { }

public class TargetTurnPlayer : ServerSubeffect
{
	public TargetTurnPlayer(TargetTurnPlayerData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.PlayerTargets.Add(Game.TurnPlayer);
		return Task.FromResult(ResolutionInfo.Next);
	}
}
