namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class NoUniqueCopyExists : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> !InitializationContext.game.BoardHasCopyOf(InitializationContext.source);
	}
}