using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities
{
	public abstract class TriggerContextualCardIdentityBase : TriggerContextualLeafIdentityBase<IGameCardInfo>,
		IIdentity<Space>
	{
		Space IIdentity<Space>.From(IResolutionContext? context, IResolutionContext? secondaryContext)
			=> Item.Position
			?? throw (Item.Location == Gamestate.Locations.Location.Board
				? new NullSpaceOnBoardException(Item)
				: new IllDefinedException());
	}
}