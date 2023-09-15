using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{

	public class CountCards : ContextualParentIdentityBase<int>
	{
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new ManyCards.All();

		[JsonProperty]
		public IRestriction<GameCardBase> cardRestriction = new Restrictions.Gamestate.AlwaysValid();

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

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.From(context, secondaryContext).Count(c => cardRestriction.IsValid(c, default));
	}
}