using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Shared;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Limit : ContextualParentIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> limit;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<IGameCard>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			limit.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCard> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> CollectionsHelper.Shuffle(cards.From(context, secondaryContext))
				.Take(limit.From(context, secondaryContext))
				.ToArray();
	}
}