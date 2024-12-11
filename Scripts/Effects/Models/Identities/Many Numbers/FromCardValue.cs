using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyNumbers;

public class FromCardValue : ContextualParentIdentityBase<IReadOnlyCollection<int>>
{
	#nullable disable
	[JsonProperty]
	public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards = new ManyCards.All();
	[JsonProperty]
	public IRestriction<IGameCardInfo> cardRestriction = new Restrictions.Gamestate.AlwaysValid();
	[JsonProperty(Required = Required.Always)]
	public CardValue cardValue;
	#nullable restore

	public override void Initialize(InitializationContext initializationContext)
	{
		base.Initialize(initializationContext);

		cards = new ManyCards.Restricted()
		{
			cards = cards,
			cardRestriction = cardRestriction
		};

		cards.Initialize(initializationContext);
		cardValue.Initialize(initializationContext);
	}

	protected override IReadOnlyCollection<int> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
	{
		var card = this.cards.From(context, secondaryContext)
			?? throw new InvalidOperationException();
		return card.Select(cardValue.GetValueOf).ToArray();
	}
}