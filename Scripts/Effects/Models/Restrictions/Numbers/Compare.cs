using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Relationships;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Numbers
{
	public class Compare : RestrictionBase<int>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> other;
		[JsonProperty(Required = Required.Always)]
		public INumberRelationship comparison;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(int item, IResolutionContext context)
			=> comparison.Compare(item, other.From(context));
	}

	public class Positive : Compare
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			other ??= Identities.Numbers.Constant.Zero;
			comparison ??= new Relationships.Numbers.GreaterThan();
			base.Initialize(initializationContext);
		}
	}
}