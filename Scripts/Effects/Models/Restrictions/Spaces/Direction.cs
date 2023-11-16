using System;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class Direction : SpaceRestrictionBase
	{
		//One of these should be non-null. The other one will be replaced by the space to be tested
		[JsonProperty]
		public IIdentity<Space> from;
		[JsonProperty]
		public IIdentity<Space> to;

		[JsonProperty]
		public IIdentity<Space> direction = new Identities.Spaces.TargetIndex();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from?.Initialize(initializationContext);
			to?.Initialize(initializationContext);
			direction.Initialize(initializationContext);

			if (from != null && to != null) throw new ArgumentException("Specified both 'from' and 'to' spaces in direction restriction - what direction are we testing?");
			if (from == null && to == null) throw new ArgumentException("Specified neither 'from' nor 'to' spaces in direction restriction - what direction are we testing?");
		}

		protected override bool IsValidLogic(Space? space, IResolutionContext context)
		{
			var origin = from?.From(context) ?? space;
			var destination = to?.From(context) ?? space;

			return origin.DirectionFromThisTo(destination) == direction.From(context);
		}
	}
}