using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class ConnectedTo : SpaceRestrictionBase
	{
		#nullable disable
		[JsonProperty]
		public IIdentity<Space> space;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> anyOfTheseSpaces;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Space> byRestriction;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (AllNull(space, spaces, anyOfTheseSpaces))
				throw new System.ArgumentNullException("spaces", "Failed to provide spaces for space restriction elements");

			spaces ??= new Identities.ManySpaces.Multiple() { spaces = new IIdentity<Space>[] { space } };
			spaces.Initialize(initializationContext);
			anyOfTheseSpaces?.Initialize(initializationContext);

			byRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? space, IResolutionContext context)
		{
			return spaces.From(context)
				.All(s => Kompas.Gamestate.Locations.Models.Board.AreConnectedBySpaces(s, space, byRestriction, context));
		}
	}
}