using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CardAfter : TriggerContextualCardIdentityBase
	{
		public bool secondaryCard;

		protected override IGameCardInfo? AbstractItemFrom(IEventContext context)
			=> secondaryCard
				? context.SecondaryCardAfter
				: context.MainCardAfter;
	}
}