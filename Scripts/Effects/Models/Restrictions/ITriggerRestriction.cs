namespace Kompas.Effects.Models.Restrictions
{
	public interface ITriggerRestriction : IRestriction<TriggeringEventContext>
	{
		public bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext);
	}
}