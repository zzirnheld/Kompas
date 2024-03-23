using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class NoUniqueCopyExists : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
		{
			var card = InitializationContext.source
				?? throw new IllDefinedException();
			return !InitializationContext.game.BoardHasCopyOf(card);
		}

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> true;
	}
}