using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class Corner : SpaceRestrictionBase
	{
		protected override bool IsValidLogic(Space toTest, IResolutionContext context)
			=> toTest.IsCorner;
	}
}