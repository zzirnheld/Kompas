using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class CompareDistance : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> distanceTo;
		#nullable restore

		[JsonProperty]
		public bool closest = true;

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
			distanceTo.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var spaces = this.spaces.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			var dest = distanceTo.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			var tuples = spaces
				.Select(s => (s, s.DistanceTo(dest)))
				.OrderBy(tuple => tuple.Item2);
			if (!tuples.Any()) return Array.Empty<Space>();
			
			int dist = tuples.First().Item2;
			return tuples.Where(tuple => tuple.Item2 == dist).Select(tuple => tuple.s).ToList();
		}
		
	}
}