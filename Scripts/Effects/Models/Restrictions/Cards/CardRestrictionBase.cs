using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public abstract class CardRestrictionBase : RestrictionBase<IGameCardInfo>, IRestriction<Space>, IRestriction<(Space s, IPlayer p)>
	{
		public bool IsValid(Space item, IResolutionContext context)
			=> IsValid(InitializationContext.game.Board.GetCardAt(item), context);
		public bool IsValid((Space s, IPlayer p) item, IResolutionContext context)
			=> IsValid(item.s, context);
	}
}