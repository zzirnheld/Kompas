using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Attacker : TriggerContextualCardIdentityBase
	{
		protected override IGameCardInfo AbstractItemFrom(IEventContext contextToConsider)
			=> GetAttack(contextToConsider).attacker;
	}
}