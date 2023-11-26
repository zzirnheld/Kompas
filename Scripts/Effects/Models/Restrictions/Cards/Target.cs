using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Target : CardRestrictionBase
	{
		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			if (card == null) return false;
			var effect = InitializationContext.effect ?? throw new IllDefinedException();
			return effect.CardTargets.Contains(card.Card);
		}
	}
}