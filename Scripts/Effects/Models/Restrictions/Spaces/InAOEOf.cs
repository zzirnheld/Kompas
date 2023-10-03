using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using System;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class InAOEOf : SpaceRestrictionBase
	{
		[JsonProperty]
		public IIdentity<IGameCard> card;
		[JsonProperty]
		public IRestriction<IGameCard> cardRestriction; //Used to restrict anyOf. If non-null, but anyOf is null, will make anyOf default to All()
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCard>> anyOf;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCard>> allOf;

		[JsonProperty]
		public IIdentity<int> minAnyOfCount = Identities.Numbers.Constant.One;

		[JsonProperty]
		public IIdentity<Space> alsoInAOE;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			if (cardRestriction != null && anyOf == null) anyOf = new Identities.ManyCards.All();

			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			cardRestriction?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);
			allOf?.Initialize(initializationContext);

			if (AllNull(card, cardRestriction, anyOf, allOf))
				throw new System.ArgumentNullException(nameof(card), $"Provided no card/s to be in AOE of for {initializationContext.source?.CardName}");

			minAnyOfCount.Initialize(initializationContext);

			alsoInAOE?.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var alsoInAOE = this.alsoInAOE?.From(context);
			bool IsValidAOE(IGameCard card)
			{
				return card.SpaceInAOE(space)
					&& (alsoInAOE == null || card.SpaceInAOE(alsoInAOE));
			}
			if (card != null && !ValidateCard(IsValidAOE, context)) return false;
			if (anyOf != null && !ValidateAnyOf(IsValidAOE, context)) return false;
			if (allOf != null && !ValidateAllOf(IsValidAOE, context)) return false;
			return true;
		}

		private bool ValidateCard(Func<IGameCard, bool> isValidCard, IResolutionContext context)
			=> isValidCard(card.From(context));

		private bool ValidateAnyOf(Func<IGameCard, bool> isValidCard, IResolutionContext context) 
		{
			IEnumerable<IGameCard> cards = anyOf.From(context);
			if (cardRestriction != null) cards = cards.Where(c => cardRestriction.IsValid(c, context));

			return minAnyOfCount.From(context) <= cards.Count(c => isValidCard(c));
		}

		private bool ValidateAllOf(Func<IGameCard, bool> isValidCard, IResolutionContext context)
			=> allOf.From(context).All(isValidCard);
	}
}