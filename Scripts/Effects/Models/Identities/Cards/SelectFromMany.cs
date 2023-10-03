using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Selectors;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class SelectFromMany : ContextualParentIdentityBase<IGameCard>
	{
		[JsonProperty]
		public ISelector<IGameCard> selector = new RandomCard();
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCard>> cards = new ManyCards.All();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IGameCard AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> selector.Select(cards.From(context, secondaryContext));
	}
}