using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public abstract class CardRestrictionElement : RestrictionBase<GameCardBase>, IRestriction<Space>, IRestriction<(Space s, Player p)>
	{
		public bool IsValid(Space item, IResolutionContext context)
			=> IsValid(InitializationContext.game.BoardController.GetCardAt(item), context);
		public bool IsValid((Space s, Player p) item, IResolutionContext context)
			=> IsValid(item.s, context);
	}
}