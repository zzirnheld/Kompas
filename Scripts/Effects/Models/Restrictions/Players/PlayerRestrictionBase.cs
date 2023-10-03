using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public abstract class PlayerRestrictionBase : RestrictionBase<Player>, IRestriction<IGameCard>, IRestriction<(Space s, Player p)>
	{
		public bool IsValid(IGameCard item, IResolutionContext context)
			=> IsValid(item.ControllingPlayer, context);

		public bool IsValid((Space s, Player p) item, IResolutionContext context)
			=> IsValid(item.p, context);
	}
}