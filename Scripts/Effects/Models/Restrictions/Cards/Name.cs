using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Name : CardRestrictionBase
	{
		[JsonProperty]
		public string nameIs;
		[JsonProperty]
		public string nameIncludes;

		[JsonProperty]
		public IIdentity<IGameCard> sameAs;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			sameAs.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IGameCard card, IResolutionContext context)
		{
			if (nameIs != null && card.CardName != nameIs) return false;
			if (nameIncludes != null && !card.CardName.Contains(nameIncludes)) return false;
			if (sameAs != null && card.CardName != sameAs.From(context).CardName) return false;

			return true;
		}
	}

	public class DistinctName : CardRestrictionBase
	{
		public IIdentity<IGameCard> from = new Identities.Cards.ThisCardNow();
		public IIdentity<IReadOnlyCollection<IGameCard>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			cards?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IGameCard card, IResolutionContext context)
		{
			if (cards == default) return from.From(context).CardName != card.CardName;

			return cards.From(context)
				.Select(c => c.CardName)
				.All(name => name != card.CardName);
		}
	}

	public class Unique : CardRestrictionBase
	{
		protected override bool IsValidLogic(IGameCard item, IResolutionContext context)
			=> item.Unique;
	}
}