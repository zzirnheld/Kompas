using System.Linq;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class EffectAlreadyTriggered : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.StackController.StackEntries.Any(e => e == InitializationContext.effect);

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> IsValid(context, dummyContext);
	}
}