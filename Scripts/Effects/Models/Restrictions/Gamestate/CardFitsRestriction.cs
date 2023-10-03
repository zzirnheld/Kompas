using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class CardFitsRestriction : TriggerGamestateRestrictionBase
	{
		[JsonProperty]
		public IIdentity<IGameCard> card;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCard>> anyOf;
		
		[JsonProperty(Required = Required.Always)]
		public IRestriction<IGameCard> cardRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);

			if (AllNull(card, anyOf)) throw new System.ArgumentException($"No card to check against restriction in {initializationContext.effect}");
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
		{
			var contextToConsider = ContextToConsider(context, secondaryContext);
			bool IsValidCard(IGameCard c) => cardRestriction.IsValid(c, contextToConsider);

			if (card != null && !IsValidCard(FromIdentity(card, context, secondaryContext))) return false;
			if (anyOf != null && !FromIdentity(anyOf, context, secondaryContext).Any(IsValidCard)) return false;

			return true;
		}

		protected virtual IdentityType FromIdentity<IdentityType>
			(IIdentity<IdentityType> identity, TriggeringEventContext triggeringEventContext, IResolutionContext resolutionContext)
			=> identity.From(triggeringEventContext, resolutionContext);

		public override string ToString()
		{
			return $"{card} or {anyOf} must be {cardRestriction}";
		}
	}
}