using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Defender : TriggerContextualCardIdentityBase
	{
		protected override IGameCardInfo AbstractItemFrom(IEventContext contextToConsider)
			=> GetAttack(contextToConsider).defender;
	}
}