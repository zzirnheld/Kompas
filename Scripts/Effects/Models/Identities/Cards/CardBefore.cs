using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CardBefore : TriggerContextualCardIdentityBase
	{
		[JsonProperty]
		public bool secondaryCard = false;

		protected override IGameCard AbstractItemFrom(TriggeringEventContext context)
			=> secondaryCard
				? context.secondaryCardInfoBefore
				: context.mainCardInfoBefore;
	}
}