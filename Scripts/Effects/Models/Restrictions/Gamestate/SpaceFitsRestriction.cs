using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class SpacesFitRestriction : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Space> spaceRestriction;
		#nullable restore
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> spaces = new Identities.ManySpaces.All();

		[JsonProperty]
		public bool any = false;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
			spaceRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IResolutionContext context)
		{
			var spacesItem = spaces.From(context)
				?? throw new InvalidOperationException();
			return any
				? spacesItem.Any(s => spaceRestriction.IsValid(s, context))
				: spacesItem.All(s => spaceRestriction.IsValid(s, context));
		}

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}

	public class SpaceFitsRestriction : SpacesFitRestriction
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> space;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			spaces = new Identities.ManySpaces.Multiple() { spaces = new[] { space } };
			base.Initialize(initializationContext);
		}
	}
}