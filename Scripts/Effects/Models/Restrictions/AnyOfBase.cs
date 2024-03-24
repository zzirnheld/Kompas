using System.Linq;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions
{
	public abstract class AnyOfBase<RestrictedType> : AnyOfBase<RestrictedType, IRestriction<RestrictedType>>
	{ }

	public abstract class AnyOfBase<RestrictedType, ElementRestrictionType>
		: RestrictionBase<RestrictedType>
			where ElementRestrictionType : IRestriction<RestrictedType>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public ElementRestrictionType[] elements;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var restriction in elements) restriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType? item, IResolutionContext context)
			=> elements.Any(r => r.IsValid(item, context));
	}
}