using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class CardsMatch : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCardInfo> card;
		[JsonProperty]
		public IIdentity<IGameCardInfo> other;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> anyOf;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);

			other?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);

			if (AllNull(other, anyOf)) throw new System.ArgumentNullException("other", "No card to compare the card to in trigger restriction element");
		}

		protected override bool IsValidLogic(IResolutionContext context)
		{
			var first = card.From(context)?.Card;

			if (other != null && first != other.From(context)?.Card) return false;
			if (anyOf != null)
			{
				var cards = anyOf.From(context);
				if (cards == null) throw new InvalidOperationException();
				if (!cards.Any(c => c?.Card == first)) return false;
			}

			return true;
		}

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}
}