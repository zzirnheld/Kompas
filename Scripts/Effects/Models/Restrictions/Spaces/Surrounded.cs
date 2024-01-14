using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class Surrounded : SpaceRestrictionBase
	{
		protected override bool IsValidLogic(Space? toTest, IResolutionContext context)
			=> toTest != null
			&& InitializationContext.game.Board.Surrounded(toTest);
	}
}