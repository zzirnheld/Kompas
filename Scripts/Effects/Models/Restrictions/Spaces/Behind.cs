using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class Behind : SpaceRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCard> card;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			return card.From(context).SpaceBehind(space);
		}
	}
}