using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetAllData : CardTargetData { }

public class TargetAll : CardTarget
{
	public TargetAll(TargetAllData data) : base(data) { }

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
	{
		var (_, targets) = DeterminePossibleTargets(context);
		return !targets.Any();
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		//check what targets there are now, before you add them, to not mess with NotAlreadyTarget restriction
		//because Linq executes lazily, it would otherwise add the targets, then re-execute the query and not find any
		var (_, targets) = DeterminePossibleTargets(resolution.Context);
		foreach (var t in targets) resolution.AddTarget(t);

		if (targets.Any()) return Task.FromResult(ResolutionInfo.Next);
		else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
	}
}