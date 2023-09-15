using System.Collections.Generic;
using System.Linq;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class CompareDistance : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> distanceTo;

		[JsonProperty]
		public bool closest = true;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
			distanceTo.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var tuples = spaces.From(context, secondaryContext)
				.Select(s => (s, s.DistanceTo(distanceTo.From(context, secondaryContext))))
				.OrderBy(tuple => tuple.Item2);
			if (tuples.Count() == 0) return tuples.Select(tuple => tuple.s).ToList();
			
			int dist = tuples.First().Item2;
			return tuples.Where(tuple => tuple.Item2 == dist).Select(tuple => tuple.s).ToList();
		}
		
	}
}