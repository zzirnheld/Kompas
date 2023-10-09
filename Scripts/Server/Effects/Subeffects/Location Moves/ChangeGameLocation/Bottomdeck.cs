using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace Kompas.Server.Effects.Subeffects
{
	public class Bottomdeck : ChangeGameLocation
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext) == null;
		protected override CardLocation destination => CardLocation.Deck;

		protected override void ChangeLocation(GameCard card) => card.Bottomdeck(card.Owner, Effect);
	}
}