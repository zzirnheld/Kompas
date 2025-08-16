using System.Linq;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class LoopWhileHaveTargetsData : LoopData
{
	[JsonProperty]
	public bool delete = false;

	[JsonProperty]
	public int remainingTargets = 0;

	[JsonProperty]
	public IIdentity<int>? leaveRemainingTargets;
}

public class LoopWhileHaveTargets : Loop<LoopWhileHaveTargetsData>
{
	private readonly bool delete;
	private readonly int remainingTargets;
	private readonly IIdentity<int> leaveRemainingTargets;

	public LoopWhileHaveTargets(LoopWhileHaveTargetsData data) : base(data)
	{
		delete = data.delete;
		remainingTargets = data.remainingTargets;
		leaveRemainingTargets = data.leaveRemainingTargets
			?? new Kompas.Effects.Models.Identities.Numbers.Constant() { constant = remainingTargets };
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		leaveRemainingTargets.Initialize(DefaultInitializationContext);
	}

	protected override bool LoopContinuation(ServerEffectResolution resolution)
	{
		//if we're deleting and there's something to delete, delete it.
		if (delete && resolution.Context.CardTargets.Any()) resolution.RemoveTarget(GetCardTarget(resolution.Context));
		//after any delete that might have happened, check if there's still targets
		return resolution.Context.CardTargets.Count > leaveRemainingTargets.From(resolution.Context);
	}
}