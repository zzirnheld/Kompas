using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CauseAfter : TriggerContextualCardIdentityBase
	{
		protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.CauseCardInfoAfter;
	}
}