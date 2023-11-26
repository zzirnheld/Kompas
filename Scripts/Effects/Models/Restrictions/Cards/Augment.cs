using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	//TODO: this can probably be merged with/generalized to a "card is" sort of restriction,
	// where there's an additional IIdentity<IGameCard> that defines the card to actually be tested in terms of the incoming card?
	public abstract class AugmentRestrictionBase : CardRestrictionBase
	{
		[JsonProperty]
		public IRestriction<IGameCardInfo>? cardRestriction;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>>? manyCards;
		[JsonProperty]
		public IIdentity<IGameCardInfo>? singleCard;

		/// <summary>
		/// Returns a predicate that tests the test card with the following order of priorities:
		/// If the cardRestriction is defined, checks that the test card fits that restriction.
		/// If no CardRestriction is defined, but a list of cards is defined, checks if the test card is one of those cards.
		/// If neither is defined, but a single card identity is defined, checks if the test card is that card.
		/// </summary>
		protected Func<IGameCardInfo?, bool> IsValidAug(IResolutionContext context) => card =>
		{
			if (cardRestriction != null) return cardRestriction.IsValid(card, context);
			if (manyCards != null)
			{
				var cards = manyCards.From(context, context)
					?? throw new InvalidOperationException();
				return card != null && cards.Contains(card);
			}
			if (singleCard != null) return singleCard.From(context, context) == card;
			throw new System.ArgumentNullException(nameof(card), $"No augment provided for {this.GetType()} CardRestrictionElement");
		};

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);

			if (AllNull(cardRestriction, singleCard, manyCards))
				throw new System.ArgumentNullException("augment", $"No augment provided for {this.GetType()} CardRestrictionElement");

			cardRestriction?.Initialize(initializationContext);
			manyCards?.Initialize(initializationContext);
			singleCard?.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}
	}

	public class HasAugment : AugmentRestrictionBase
	{
		[JsonProperty]
		public bool all = false; //default to any

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			if (card == null) return false;
			return all
				? card.Augments.All(IsValidAug(context))
				: card.Augments.Any(IsValidAug(context));
		}
	}

	public class Augments : AugmentRestrictionBase
	{
		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card != null && IsValidAug(context)(card.AugmentedCard);
	}
}