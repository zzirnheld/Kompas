using System.Linq;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class EffectAlreadyTriggered : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.StackEntries.Any(e => e == InitializationContext.effect);
	}
}