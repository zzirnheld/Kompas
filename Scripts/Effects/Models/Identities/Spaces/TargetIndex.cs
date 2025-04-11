using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces;

public class TargetIndex : EffectContextualLeafIdentityBase<Space>
{
	[JsonProperty]
	public int index = -1;

	protected override Space? AbstractItemFrom(IResolutionContext toConsider)
		=> toConsider.GetSpaceTarget(index);
}