using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTargetsControllerData : SubeffectData { }

public class TargetTargetsController : ServerSubeffect
{
	public TargetTargetsController(TargetTargetsControllerData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null) throw new NullCardException(TargetWasNull);
		resolution.Context.PlayerTargets.Add(GetCardTarget(resolution.Context).ControllingPlayer);
		return Task.FromResult(ResolutionInfo.Next);
	}
}