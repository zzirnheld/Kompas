using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class AOEContains : CardRestrictionBase
	{
		//If you want to specify the cards that you want to have at least one in AOE using an identity, you can use this one.
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards = new Identities.ManyCards.Board();
		//If you just wanna restrict which of the cards on board have to fit, you can use this one.
		[JsonProperty]
		public IRestriction<IGameCardInfo> cardRestriction = new Gamestate.AlwaysValid();

		[JsonProperty]
		public bool all = false; //false = any;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			if (card == null) return false;
			var wantedCards = cards.From(context)
				?.Where(c => cardRestriction.IsValid(c, context))
				?? throw new InvalidOperationException();

			return all
				? wantedCards.All(card.CardInAOE)
				: wantedCards.Any(card.CardInAOE);
		}
	}
}