using System.Linq;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class EffectAlreadyTriggered : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.StackController.StackEntries.Any(e => e == InitializationContext.effect);

		public override bool IsStillValidTriggeringContext(IEventContext context)
			=> IsValid(IResolutionContext.NotResolving(context));
	}
}