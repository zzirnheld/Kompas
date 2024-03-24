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
			return !card.Unique || !InitializationContext.game.BoardHasCopyOf(card);
		}

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}
}