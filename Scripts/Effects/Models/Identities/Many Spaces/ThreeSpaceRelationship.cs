using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Relationships.Spaces;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	/// <summary>
	/// Spaces where they are in some defined relationship with respect to the other two defined spaces.
	/// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
	/// </summary>
	public class ThreeSpaceRelationship : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> firstSpace;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> secondSpace;

		[JsonProperty(Required = Required.Always)]
		public IThreeSpaceRelationship thirdSpaceRelationship;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstSpace.Initialize(initializationContext);
			secondSpace.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			Space first = firstSpace.From(context, secondaryContext);
			Space second = secondSpace.From(context, secondaryContext);
			return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
		}
	}
}