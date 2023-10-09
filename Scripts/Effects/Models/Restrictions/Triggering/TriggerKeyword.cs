using System.Linq;
using Kompas.Cards.Loading;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Triggering
{
	public class TriggerKeyword : TriggerRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public string keyword;

		private IRestriction<TriggeringEventContext> [] elements;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			elements = CardRepository.InstantiateTriggerKeyword(keyword);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> elements.All(tre => tre.IsValid(context, secondaryContext));
	}
}