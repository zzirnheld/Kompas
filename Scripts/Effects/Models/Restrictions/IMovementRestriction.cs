using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions;

public interface IMovementRestriction : IRestriction<Space>
{
	public static IMovementRestriction CreateDefault()
		=> new Spaces.MovementRestriction();

	public bool WouldBeValidNormalMoveInOpenGamestate(Space space);

	public int GetMovementCost(Space from, Space to, IGame game);
}

public static class MovementRestrictionExtensions
{
	//TODO: take into account stuff like Shape - possibly a custom predicate here?
	public static MovePath Path(this IMovementRestriction restriction, Space from, Space to, IResolutionContext context)
	{
		return Space.ShortestPathBetween(from, to,
			s => restriction.IsValid(s, context));
	}
}