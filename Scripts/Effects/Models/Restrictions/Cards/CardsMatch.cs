using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class CardsMatch : CardRestrictionBase
	{
		[JsonProperty]
		public IIdentity<IGameCardInfo>? card;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>>? cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			cards?.Initialize(initializationContext);
			if (AllNull(card, cards)) throw new NullReferenceException("Need to have a card or cards to match!");
		}

		protected override bool IsValidLogic (IGameCardInfo? item, IResolutionContext context)
		{
			if (card != null) return item?.Card == card.From(context).Card;
			else return cards?.From(context).Any(c => c.Card == item?.Card) ?? false;
		}
	}
}