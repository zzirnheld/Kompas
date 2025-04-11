using System.Linq;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class LoopWhileHaveTargets : Loop
{
	[JsonProperty]
	public bool delete = false;

	[JsonProperty]
	public int remainingTargets = 0;
	#nullable disable
	[JsonProperty]
	public IIdentity<int> leaveRemainingTargets;
	#nullable restore

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		leaveRemainingTargets ??= new Kompas.Effects.Models.Identities.Numbers.Constant() { constant = remainingTargets };
	}

	protected override bool LoopContinuation(ServerEffectResolution resolution)
	{
		//if we're deleting and there's something to delete, delete it.
		if (delete && resolution.Context.CardTargets.Any()) resolution.RemoveTarget(GetCardTarget(resolution.Context));
		//after any delete that might have happened, check if there's still targets
		return resolution.Context.CardTargets.Count > leaveRemainingTargets.From(resolution.Context);
	}
}