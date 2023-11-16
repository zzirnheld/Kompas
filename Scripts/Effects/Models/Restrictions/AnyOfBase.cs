using System.Linq;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions
{
	public abstract class AnyOfBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<RestrictedType>[] elements;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var restriction in elements) restriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType? item, IResolutionContext? context)
			=> elements.Any(r => r.IsValid(item, context));
	}
}