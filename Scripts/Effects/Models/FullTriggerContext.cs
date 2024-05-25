namespace Kompas.Effects.Models
{
	public class FullTriggerContext
	{
		public string TriggeringCondition { get; }

		public IEventContext Context { get; }

		public FullTriggerContext(string triggeringCondition, IEventContext context)
		{
			TriggeringCondition = triggeringCondition;
			Context = context;
		}
	}
}