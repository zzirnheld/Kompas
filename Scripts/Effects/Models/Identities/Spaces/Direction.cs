using System;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class Direction : ContextualParentIdentityBase<Space>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> from;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> to;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			to.Initialize(initializationContext);
		}

		protected override Space? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var origin = from.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			var dest = to.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return origin.DirectionFromThisTo(dest);
		}
	}
}