using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Target : CardRestrictionBase
	{
		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> InitializationContext.effect.CardTargets.Contains(card.Card);
	}
}