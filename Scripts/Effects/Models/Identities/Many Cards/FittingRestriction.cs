using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Restricted : ContextualParentIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards = new ManyCards.All();
		[JsonProperty(Required = Required.Always)]

		public IRestriction<IGameCardInfo> cardRestriction;

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

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.From(context, secondaryContext).Where(c => cardRestriction.IsValid(c, context)).ToArray();
	}
}