using System;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class Multiply : ContextualParentIdentityBase<Space>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> toMultiply;
		#nullable restore

		[JsonProperty]
		public int multiplier = 1;
		[JsonProperty]
		public int xMultiplier = 1;
		[JsonProperty]
		public int yMultiplier = 1;

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toMultiply.Initialize(initializationContext);
		}

		protected override Space? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var space = toMultiply.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			space *= multiplier;
			space.x *= xMultiplier;
			space.y *= yMultiplier;
			return space;
		}
	}
}