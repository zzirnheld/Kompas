using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class Distance : ContextualParentIdentityBase<int>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> firstSpace;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> secondSpace;

		[JsonProperty]
		public IRestriction<Space> throughRestriction;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstSpace.Initialize(initializationContext);
			secondSpace.Initialize(initializationContext);

			throughRestriction?.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			Space first = firstSpace.From(context, secondaryContext);
			Space second = secondSpace.From(context, secondaryContext);

			if (first == null || second == null) return -1;

			if (throughRestriction == null) return first.DistanceTo(second);

			var contextToConsider = base.secondaryContext ? secondaryContext : context;
			bool through(Space s) => throughRestriction.IsValid(s, contextToConsider);
			return Gamestate.Locations.Models.Board.ShortestPath(first, second, through);
		}
	}
}