using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Effects.Models.Identities.Cards;

public class CauseBefore : TriggerContextualCardIdentityBase
{
	protected override IGameCardInfo? AbstractItemFrom(IEventContext contextToConsider)
		=> contextToConsider.CauseCardBefore;
}