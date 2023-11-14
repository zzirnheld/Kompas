using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public abstract class PlayerRestrictionBase : RestrictionBase<IPlayer>, IRestriction<IGameCardInfo>, IRestriction<(Space s, IPlayer p)>
	{
		public bool IsValid(IGameCardInfo item, IResolutionContext context)
			=> IsValid(item.ControllingPlayer, context);

		public bool IsValid((Space s, IPlayer p) item, IResolutionContext context)
			=> IsValid(item.p, context);
	}
}