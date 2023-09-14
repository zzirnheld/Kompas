using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IMovementRestriction : IRestriction<Space>
	{
		public static IMovementRestriction CreateDefault()
			=> new SpaceRestrictionElements.MovementRestriction();

		public bool WouldBeValidNormalMoveInOpenGamestate(Space space);
	} //TODO add the get movement cost thing here	
}