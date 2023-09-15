using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Attacker : TriggerContextualCardIdentityBase
	{
		protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> GetAttack(contextToConsider).attacker;
	}
}