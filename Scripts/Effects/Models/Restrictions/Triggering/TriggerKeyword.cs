using System.Linq;
using Kompas.Cards.Loading;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Triggering
{
	public class TriggerKeyword : TriggerRestrictionBase
	{
		//If I wanted to remove nullable disable/enable, the pattern I would want would match the NotInitializedExceptions found in Godot code
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public string keyword;
		//Initialized after json loaded
		private ITriggerRestriction[] elements;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			elements = CardRepository.InstantiateTriggerKeyword(keyword);
		}

		protected override bool IsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> elements.All(tre => tre.IsValid(context, secondaryContext));

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> elements.All(tre => tre.IsStillValidTriggeringContext(context, dummyContext));
	}
}