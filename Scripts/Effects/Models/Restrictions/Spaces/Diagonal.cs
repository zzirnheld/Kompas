using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class SameDiagonal : SpaceRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> other;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? space, IResolutionContext context)
			=> space != null
			&& (other.From(context)?.SameDiagonal(space) ?? false);
	}
}