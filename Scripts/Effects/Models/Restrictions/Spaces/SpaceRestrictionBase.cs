using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public abstract class SpaceRestrictionBase : RestrictionBase<Space>, IRestriction<IGameCard>, IRestriction<TriggeringEventContext>, IRestriction<(Space s, IPlayer p)>
	{
		public bool IsValid(IGameCard item, IResolutionContext context) => IsValid(item?.Position, context);
		public bool IsValid(TriggeringEventContext item, IResolutionContext context) => IsValid(item?.space, context);
		public bool IsValid((Space s, IPlayer p) item, IResolutionContext context) => IsValid(item.s, context);
	}
}