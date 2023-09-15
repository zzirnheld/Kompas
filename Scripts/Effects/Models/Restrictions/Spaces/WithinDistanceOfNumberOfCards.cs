using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;
using System.Linq;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class WithinDistanceOfNumberOfCards : SpaceRestrictionBase
	{
		[JsonProperty]
		public IRestriction<GameCardBase> cardRestriction = new Gamestate.AlwaysValid();

		[JsonProperty]
		public IIdentity<int> numberOfCards = Identities.Numbers.Constant.One;
		[JsonProperty]
		public IIdentity<int> distance = Identities.Numbers.Constant.One;

		[JsonProperty]
		public bool excludeSelf = true;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);
			numberOfCards.Initialize(initializationContext);
			distance.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			return InitializationContext.game.Cards
				.Where(c => c.DistanceTo(space) < distance.From(context))
				.Where(c => cardRestriction.IsValid(c, context))
				.Where(c => !excludeSelf || c != InitializationContext.source)
				.Count() >= numberOfCards.From(context);
		}
	}
}