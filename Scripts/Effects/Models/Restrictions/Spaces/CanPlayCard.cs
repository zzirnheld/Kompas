using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	/// <summary>
	/// Whether a card can be moved to that space. Presumes from effect
	/// </summary>
	public class CanPlayCard : SpaceRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCard> toPlay;

		[JsonProperty]
		public bool ignoreAdjacency;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toPlay.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var restriction = toPlay.From(context).PlayRestriction;
		
			return ignoreAdjacency
				? restriction.IsValidIgnoringAdjacency((space, InitializationContext.Controller), context)
				: restriction.IsValid((space, InitializationContext.Controller), context);
		}
	}
}