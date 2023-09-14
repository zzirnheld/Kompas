using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities
{
	public abstract class ContextlessLeafCardIdentityBase : ContextlessLeafIdentityBase<GameCardBase>, IIdentity<Space>, IIdentity<IReadOnlyCollection<GameCardBase>>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;

		IReadOnlyCollection<GameCardBase> IIdentity<IReadOnlyCollection<GameCardBase>>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> new [] { Item };
	}
}