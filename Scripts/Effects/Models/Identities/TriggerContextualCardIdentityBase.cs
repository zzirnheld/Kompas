using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities
{
	public abstract class TriggerContextualCardIdentityBase : TriggerContextualLeafIdentityBase<IGameCard>,
		IIdentity<Space>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;
	}
}