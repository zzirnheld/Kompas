using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CauseBefore : TriggerContextualCardIdentityBase
	{
		protected override IGameCard AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.cardCauseBefore;
	}
}