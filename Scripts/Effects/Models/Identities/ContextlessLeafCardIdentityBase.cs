using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities
{
	public abstract class ContextlessLeafCardIdentityBase : ContextlessLeafIdentityBase<IGameCard>, IIdentity<Space>, IIdentity<IReadOnlyCollection<IGameCard>>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;

		IReadOnlyCollection<IGameCard> IIdentity<IReadOnlyCollection<IGameCard>>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> new [] { Item };
	}
}