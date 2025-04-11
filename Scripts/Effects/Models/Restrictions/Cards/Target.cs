using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.Cards;

public class Target : CardRestrictionBase
{
	protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
	{
		if (card == null) return false;
		return context.CardTargets.Contains(card.Card);
	}
}