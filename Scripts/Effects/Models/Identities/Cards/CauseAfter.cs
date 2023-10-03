using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CauseAfter : TriggerContextualCardIdentityBase
	{
		protected override IGameCard AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.CauseCardInfoAfter;
	}
}