using System;
using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Selectors;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class SelectFromMany : ContextualParentIdentityBase<IGameCardInfo>
	{
		[JsonProperty]
		public ISelector<IGameCardInfo> selector = new RandomCard();
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards = new ManyCards.All();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IGameCardInfo? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var cards = this.cards.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return selector.Select(cards);
		}
	}
}