using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class Edge : SpaceRestrictionBase
	{
		protected override bool IsValidLogic(Space toTest, IResolutionContext context)
			=> toTest.IsEdge;
	}
}