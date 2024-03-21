using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Relationships;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	/// <summary>
	/// Gets the distance between the described origin point and the space to be tested,
	/// gets the described number,
	/// and compares the distance to the number with the given comparison.
	/// </summary>
	public class CompareDistance : SpaceRestrictionBase
	{
		/// <summary>
        /// Whether the distance should be through the shortest empty path.
        /// If true, will check the shortest path through empty spaces.
        /// If false, considers the pure taxicab distance.
        /// </summary>
		[JsonProperty]
		public bool shortestEmptyPath = false;
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> distanceTo;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> number;
		#nullable restore
		[JsonProperty]
		public INumberRelationship comparison = new Relationships.Numbers.Equal();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			distanceTo.Initialize(initializationContext);
			number.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? space, IResolutionContext context)
		{
			if (space == null) return false;

			var origin = distanceTo.From(context);
			if (origin == null) return false;

			int distance = shortestEmptyPath
				? InitializationContext.game.Board.ShortestEmptyPath(origin, space)
				: origin.DistanceTo(space);

			int number = this.number.From(context);

			return comparison.Compare(distance, number);
		}
	}

	public class Towards : SpaceRestrictionBase
	{
		#nullable disable
		//Whether the space to be tested's distance to the destination
		//is closer than other's distance to the destination
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> destination;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> origin;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination.Initialize(initializationContext);
			origin.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? item, IResolutionContext context)
		{
			var dest = destination.From(context);
			var orig = origin.From(context);
			if (dest == null || orig == null || item == null) return false;

			return dest.DistanceTo(item) < dest.DistanceTo(orig);
		}
	}

	public class TowardsAny : SpaceRestrictionBase
	{
		#nullable disable
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> anyDestination;
		[JsonProperty]
		public IRestriction<Space> anyDestinationRestriction;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> origin;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			anyDestination ??= new Identities.ManySpaces.Restricted() { restriction = anyDestinationRestriction };
			anyDestination.Initialize(initializationContext);
			origin.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? item, IResolutionContext context)
		{
			var destinations = anyDestination.From(context);
			var orig = origin.From(context);
			if (destinations == null || orig == null || item == null) return false;

			return destinations.Any(dest => dest.DistanceTo(item) < dest.DistanceTo(orig));
		}
	}

	public class AwayFrom : SpaceRestrictionBase
	{
		#nullable disable
		//Whether the space to be tested's distance to the destination
		//is further than other's distance to the destination
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> destination;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> origin;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination.Initialize(initializationContext);
			origin.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? item, IResolutionContext context)
		{
			var dest = destination.From(context);
			var orig = origin.From(context);
			if (dest == null || orig == null || item == null) return false;

			return dest.DistanceTo(item) > dest.DistanceTo(orig);
		}
	}
}