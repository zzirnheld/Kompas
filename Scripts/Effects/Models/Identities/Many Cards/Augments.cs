using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Augments : ContextualParentIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCardInfo> card;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> card.From(context, secondaryContext).Augments;
	}
}