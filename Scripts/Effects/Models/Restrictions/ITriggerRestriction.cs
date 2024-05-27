using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Effects.Models.Restrictions
{
	public interface ITriggerRestriction : IRestriction<IEventContext>
	{
		public bool IsStillValidTriggeringContext(IEventContext context);
	}
}