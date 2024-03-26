using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Distinct : ContextualParentIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var cards = this.cards.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return cards
				.GroupBy(c => c.CardName)
				.Select(group => group.First())
				.ToArray();
		}
	}
}