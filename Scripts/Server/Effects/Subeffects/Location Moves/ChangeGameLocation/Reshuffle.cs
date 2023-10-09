using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Reshuffle : ChangeGameLocation
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext) == null;
		protected override CardLocation Destination => CardLocation.Deck;

		protected override void ChangeLocation(GameCard card) => card.Reshuffle(card.Owner, Effect);
	}
}