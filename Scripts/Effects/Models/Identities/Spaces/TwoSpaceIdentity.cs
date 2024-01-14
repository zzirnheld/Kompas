using System;
using Kompas.Effects.Models.Relationships.Spaces;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class TwoSpaceIdentity : ContextualParentIdentityBase<Space>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> firstSpace;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> secondSpace;

		[JsonProperty(Required = Required.Always)]
		public ITwoSpaceIdentity relationship;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			firstSpace.Initialize(initializationContext);
			secondSpace.Initialize(initializationContext);
			base.Initialize(initializationContext);
		}

		protected override Space? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var first = firstSpace.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			var second = secondSpace.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return relationship.SpaceFrom(first, second);
		}
	}
}