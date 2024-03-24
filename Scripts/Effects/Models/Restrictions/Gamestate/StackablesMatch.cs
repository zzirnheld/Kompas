using System;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class StackablesMatch : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IStackable> firstStackable;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IStackable> secondStackable;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstStackable.Initialize(initializationContext);
			secondStackable.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IResolutionContext context, IResolutionContext secondaryContext)
			=> firstStackable.From(context, secondaryContext) == secondStackable.From(context, secondaryContext);

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}
}