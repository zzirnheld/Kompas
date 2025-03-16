using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards;

public class Targets : EffectContextualLeafIdentityBase<IReadOnlyCollection<IGameCardInfo>>
{
	protected override IReadOnlyCollection<IGameCardInfo>? AbstractItemFrom(IResolutionContext toConsider)
		=> toConsider.CardTargets.ToArray();
}