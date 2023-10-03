using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Distinct : ContextualParentIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<IGameCard>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCard> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.From(context, secondaryContext)
				.GroupBy(c => c.CardName)
				.Select(group => group.First())
				.ToArray();
	}
}