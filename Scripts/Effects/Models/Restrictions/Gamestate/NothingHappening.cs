namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class NothingHappening : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.StackController.NothingHappening;

		//Definitely do not reevaluate, otherwise it will always fail lol.
		public override bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> true;
	}
}