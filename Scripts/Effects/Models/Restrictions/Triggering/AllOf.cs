using System.Linq;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Triggering
{
	public class AllOf : AllOfBase<TriggeringEventContext>
	{
		protected override bool LogSoloElements => false;

		/// <summary>
		/// Reevaluates the trigger to check that any restrictions that could change between it being triggered
		/// and it being ordered on the stack, are still true.
		/// (Not relevant to delayed things, since those expire after a given number of uses (if at all), so yeah
		/// </summary>
		/// <returns></returns>
		public bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> elements.Where(elem => TriggerRestrictionBase.ReevalationRestrictions.Contains(elem.GetType()))
					.All(elem => elem.IsValid(context, IResolutionContext.Dummy(context)));
	}

	public class AnyOf : AnyOfBase<TriggeringEventContext> { }

	public class Not : TriggerRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<TriggeringEventContext> inverted;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			inverted.Initialize(initializationContext);
		}

		protected override bool IsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> !inverted.IsValid(context, secondaryContext);
	}
}