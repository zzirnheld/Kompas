using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class SameDiagonal : SpaceRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> other;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
			=> other.From(context).SameDiagonal(space);
	}
}