using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class ContextSpace : TriggerContextualLeafIdentityBase<Space>
	{
		protected override Space? AbstractItemFrom(IEventContext context)
			=> context.Space;
	}
}