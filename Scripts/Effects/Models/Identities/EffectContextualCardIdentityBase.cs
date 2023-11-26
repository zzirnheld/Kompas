using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Effects.Models.Identities
{
	public abstract class EffectContextualCardIdentityBase : EffectContextualLeafIdentityBase<IGameCardInfo>,
		IIdentity<Space>
	{
        Space? IIdentity<Space>.From(IResolutionContext? context, IResolutionContext? secondaryContext)
        {
			var item = Item;
			if (item == null) return null;
			if (item.Location != Location.Board) throw new CardNotHereException(Location.Board, item);
            return item.Position ?? throw new NullSpaceOnBoardException(item);
        }
    }
}