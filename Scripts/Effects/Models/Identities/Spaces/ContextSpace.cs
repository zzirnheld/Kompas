using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class ContextSpace : TriggerContextualLeafIdentityBase<Space>
	{
		protected override Space? AbstractItemFrom(TriggeringEventContext context)
			=> context.Space;
	}
}