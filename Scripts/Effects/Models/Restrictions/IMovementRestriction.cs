using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IMovementRestriction : IRestriction<Space>
	{
		public static IMovementRestriction CreateDefault()
			=> new Spaces.MovementRestriction();

		public bool WouldBeValidNormalMoveInOpenGamestate(Space space);

		public int MovementCost(IGame game, Space from, Space to);
	}
}