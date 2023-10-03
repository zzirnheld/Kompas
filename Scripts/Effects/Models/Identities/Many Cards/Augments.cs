using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Augments : ContextualParentIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCard> card;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCard> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> card.From(context, secondaryContext).Augments;
	}
}