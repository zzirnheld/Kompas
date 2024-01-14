using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Defender : TriggerContextualCardIdentityBase
	{
		protected override IGameCardInfo AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> GetAttack(contextToConsider).defender;
	}
}