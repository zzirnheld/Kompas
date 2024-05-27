using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Attacker : TriggerContextualCardIdentityBase
	{
		protected override IGameCardInfo AbstractItemFrom(IEventContext contextToConsider)
			=> GetAttack(contextToConsider).attacker;
	}
}