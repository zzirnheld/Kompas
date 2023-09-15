using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class StackablesMatch : TriggerGamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IStackable> firstStackable;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IStackable> secondStackable;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstStackable.Initialize(initializationContext);
			secondStackable.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> firstStackable.From(context, secondaryContext) == secondStackable.From(context, secondaryContext);
	}
}