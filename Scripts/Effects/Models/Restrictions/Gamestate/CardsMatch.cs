using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class CardsMatch : TriggerGamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase> card;
		[JsonProperty]
		public IIdentity<GameCardBase> other;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);

			other?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);

			if (AllNull(other, anyOf)) throw new System.ArgumentNullException("other", "No card to compare the card to in trigger restriction element");
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
		{
			var first = card.From(context, secondaryContext)?.Card;

			if (other != null && first != other.From(context, secondaryContext)?.Card) return false;
			if (anyOf != null && !anyOf.From(context, secondaryContext).Any(c => c?.Card == first)) return false;

			return true;
		}
	}
}