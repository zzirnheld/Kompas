using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Attacker : TriggerContextualCardIdentityBase
	{
		protected override IGameCard AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> GetAttack(contextToConsider).attacker;
	}
}