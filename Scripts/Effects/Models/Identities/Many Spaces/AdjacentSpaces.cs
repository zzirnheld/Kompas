using System.Collections.Generic;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class AdjacentSpaces : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> adjacentTo;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			adjacentTo.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
			=> adjacentTo.From(context, secondaryContext).AdjacentSpaces;
	}
}