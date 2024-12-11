using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards;

public class CardBefore : TriggerContextualCardIdentityBase
{
	[JsonProperty]
	public bool secondaryCard = false;

	protected override IGameCardInfo? AbstractItemFrom(IEventContext context)
		=> secondaryCard
			? context.SecondaryCardBefore
			: context.MainCardBefore;
}