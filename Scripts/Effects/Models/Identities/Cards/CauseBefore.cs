using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CauseBefore : TriggerContextualCardIdentityBase
	{
		protected override IGameCardInfo AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.cardCauseBefore;
	}
}