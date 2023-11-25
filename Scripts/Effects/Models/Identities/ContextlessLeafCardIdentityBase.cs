using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities
{
	public abstract class ContextlessLeafCardIdentityBase : ContextlessLeafIdentityBase<IGameCardInfo>, IIdentity<Space>, IIdentity<IReadOnlyCollection<IGameCardInfo>>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position ?? throw new NullSpaceOnBoardException(Item);

		IReadOnlyCollection<IGameCardInfo> IIdentity<IReadOnlyCollection<IGameCardInfo>>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> new [] { Item };
	}
}