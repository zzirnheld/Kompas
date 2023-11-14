using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Attacker : TriggerContextualCardIdentityBase
	{
		protected override IGameCardInfo AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> GetAttack(contextToConsider).attacker;
	}
}