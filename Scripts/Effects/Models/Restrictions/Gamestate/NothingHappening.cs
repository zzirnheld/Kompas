namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class NothingHappening : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.StackController.NothingHappening;
	}
}