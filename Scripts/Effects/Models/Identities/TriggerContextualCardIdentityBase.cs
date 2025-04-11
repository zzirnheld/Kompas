using System;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Effects.Models.Identities;

public abstract class TriggerContextualCardIdentityBase : TriggerContextualLeafIdentityBase<IGameCardInfo>,
	IIdentity<Space>
{
	Space? IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
	{
		var card = From(context, secondaryContext) ?? throw new InvalidOperationException();
		if (card.Location != Location.Board) throw new CardNotHereException(Location.Board, card);
		if (card.Position == null) throw new NullSpaceOnBoardException(card);
		return card.Position;
	}
}