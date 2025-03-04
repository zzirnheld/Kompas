using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Players;

public class TargetIndex : EffectContextualLeafIdentityBase<IPlayer>
{
	[JsonProperty]
	public int index = -1;

	protected override IPlayer? AbstractItemFrom(IResolutionContext toConsider)
		=> toConsider.GetPlayerTarget(index);
}