using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class NumberFitsRestriction : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> number;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<int> restriction;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			number.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
		}


		protected override bool IsValidLogic(TriggeringEventContext? context, IResolutionContext secondaryContext)
			=> restriction.IsValid(number.From(context, secondaryContext), secondaryContext);
	}
}