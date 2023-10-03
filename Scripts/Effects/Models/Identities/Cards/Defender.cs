using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class Defender : TriggerContextualCardIdentityBase
	{
		protected override IGameCard AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> GetAttack(contextToConsider).defender;
	}
}