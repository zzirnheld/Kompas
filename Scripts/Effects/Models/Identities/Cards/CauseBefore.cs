using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class CauseBefore : TriggerContextualCardIdentityBase
	{
		protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.cardCauseBefore;
	}
}