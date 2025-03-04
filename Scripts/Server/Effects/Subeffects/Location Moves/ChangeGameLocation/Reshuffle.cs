using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Reshuffle : ChangeGameLocation
{
	public override bool IsImpossible (IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null;
	protected override Location Destination => Location.Deck;

	protected override void ChangeLocation(GameCard card) => card.Reshuffle(card.OwningPlayer, Effect);
}