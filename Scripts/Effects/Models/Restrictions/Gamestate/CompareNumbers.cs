using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Relationships;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class CompareNumbers : TriggerGamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> firstNumber;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> secondNumber;
		[JsonProperty(Required = Required.Always)]
		public INumberRelationship comparison;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstNumber.Initialize(initializationContext);
			secondNumber.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
		{
			int first = firstNumber.From(context, secondaryContext);
			int second = secondNumber.From(context, secondaryContext);
			return comparison.Compare(first, second);
		}
	}
}