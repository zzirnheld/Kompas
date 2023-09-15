using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Selectors;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class SelectFromMany : ContextualParentIdentityBase<GameCardBase>
	{
		[JsonProperty]
		public ISelector<GameCardBase> selector = new RandomCard();
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new ManyCards.All();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override GameCardBase AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> selector.Select(cards.From(context, secondaryContext));
	}
}