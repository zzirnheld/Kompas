using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Relationships;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class CompareNumbers : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> firstNumber;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> secondNumber;
		[JsonProperty(Required = Required.Always)]
		public INumberRelationship comparison;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstNumber.Initialize(initializationContext);
			secondNumber.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IResolutionContext context)
		{
			int first = firstNumber.From(context);
			int second = secondNumber.From(context);
			return comparison.Compare(first, second);
		}

		//Because of the existence of EffectUses, this must reevaluate
		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> IsValidLogic(IResolutionContext.NotResolving(context));
	}
}