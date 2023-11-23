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
			var origin = this.distanceTo.From(context);
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
			var destination = this.destination.From(context);
			return destination.DistanceTo(item) < destination.DistanceTo(origin.From(context));
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
			var origin = this.origin.From(context);
			var destinations = anyDestination.From(context);
			return destinations.Any(destination => destination.DistanceTo(item) < destination.DistanceTo(origin));
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
			var destination = this.destination.From(context);
			return destination.DistanceTo(item) > destination.DistanceTo(origin.From(context));
		}
	}
}