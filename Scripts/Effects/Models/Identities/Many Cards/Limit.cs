using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Shared;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Limit : ContextualParentIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> limit;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			limit.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> CollectionsHelper.Shuffle(cards.From(context, secondaryContext))
				.Take(limit.From(context, secondaryContext))
				.ToArray();
	}
}