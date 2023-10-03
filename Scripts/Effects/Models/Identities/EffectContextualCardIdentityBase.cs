using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities
{
	public abstract class EffectContextualCardIdentityBase : EffectContextualLeafIdentityBase<IGameCard>, IIdentity<Space>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;
	}
}