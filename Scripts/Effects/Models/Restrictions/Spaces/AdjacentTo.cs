using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	/// <summary>
	/// Simplifies the adjacency case, even though it could just be done with "compare distance to 1".
	/// </summary>
	public class AdjacentTo : SpaceRestrictionBase
	{
		[JsonProperty]
		public IRestriction<IGameCardInfo> cardRestriction;
		[JsonProperty]
		public IIdentity<int> cardRestrictionMinimum = Identities.Numbers.Constant.One;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> anyOfTheseCards;
		[JsonProperty]
		public IIdentity<IGameCardInfo> card;
		[JsonProperty]
		public IIdentity<Space> space;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction?.Initialize(initializationContext);
			anyOfTheseCards?.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			space?.Initialize(initializationContext);
			if (AllNull(card, space, anyOfTheseCards, cardRestriction))
				throw new System.NotImplementedException($"Forgot to provide a space or card to be adjacent to " +
					$"in the effect of {InitializationContext.source}");
			else if (MultipleNonNull(card, space, anyOfTheseCards, cardRestriction))
				throw new System.NotImplementedException($"Provided both a space and a card to be adjacent to " +
					$"in the effect of {InitializationContext.source}");
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(Space toTest, IResolutionContext context)
		{
			if (cardRestriction != null)
				return toTest.AdjacentSpaces
					.Select(InitializationContext.game.Board.GetCardAt)
					.Count(c => cardRestriction.IsValid(c, context))
					>= cardRestrictionMinimum.From(context);
			else if (anyOfTheseCards != null)
				return anyOfTheseCards
					.From(context)
					.Any(c => c.IsAdjacentTo(toTest));
			else if (card != null)
				return card
					.From(context)
					.IsAdjacentTo(toTest);
			else if (space != null)
				return space
					.From(context)
					.IsAdjacentTo(toTest);
			else throw new System.NotImplementedException($"You forgot to account for some weird case for {InitializationContext.source}");
		}
	}
}