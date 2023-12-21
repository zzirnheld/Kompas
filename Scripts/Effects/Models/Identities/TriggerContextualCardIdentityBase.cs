using System;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Effects.Models.Identities
{
	public abstract class TriggerContextualCardIdentityBase : TriggerContextualLeafIdentityBase<IGameCardInfo>,
		IIdentity<Space>
	{
		Space? IIdentity<Space>.From(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			var item = Item ?? throw new InvalidOperationException();
			if (item.Location != Location.Board) throw new CardNotHereException(Location.Board, item);
			if (item.Position == null) throw new NullSpaceOnBoardException(item);
			return item.Position;
		}
	}
}