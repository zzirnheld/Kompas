using System.Linq;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Triggering
{
	public class AllOf : AllOfBase<TriggeringEventContext>, ITriggerRestriction
	{
		protected override bool LogSoloElements => false;

		/// <summary>
		/// Reevaluates the trigger to check that any restrictions that could change between it being triggered
		/// and it being ordered on the stack, are still true.
		/// (Not relevant to delayed things, since those expire after a given number of uses (if at all), so yeah
		/// </summary>
		/// <returns></returns>
		public bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> elements.All(elem => IsStillValidTriggeringContext(context, dummyContext));
	}

	public class AnyOf : AnyOfBase<TriggeringEventContext>, ITriggerRestriction
	{
		public bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> elements.Any(elem => IsStillValidTriggeringContext(context, dummyContext));
	}

	public class Not : TriggerRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public ITriggerRestriction inverted;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			inverted.Initialize(initializationContext);
		}

		//NOTE: We can't just use IsStillValidTriggeringContext because that function assumes that the restriction previously evaluated to TRUE,
		//and the whole point of Not is that we already know that inverted evaluated to false
		public override bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> IsValidContext(context, dummyContext);

		protected override bool IsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> !inverted.IsValid(context, secondaryContext);
	}
}