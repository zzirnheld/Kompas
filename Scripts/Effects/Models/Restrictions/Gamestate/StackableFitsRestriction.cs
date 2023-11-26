using System;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class StackableFitsRestriction : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<IStackable> restriction;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IStackable> stackable;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
			stackable.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(TriggeringEventContext? context, IResolutionContext secondaryContext)
		{
			var item = stackable.From(IResolutionContext.Dummy(context), secondaryContext);
			return restriction.IsValid(item, ContextToConsider(context, secondaryContext));
		}
	}
}