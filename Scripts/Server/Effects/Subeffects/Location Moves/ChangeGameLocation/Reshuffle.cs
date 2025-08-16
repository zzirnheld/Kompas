using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ReshuffleData : ChangeGameLocationData { }

public class Reshuffle : ChangeGameLocation<ReshuffleData>
{
	public Reshuffle(ReshuffleData data) : base(data) { }

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null;
	protected override Location Destination => Location.Deck;

	protected override void ChangeLocation(GameCard card, IServerResolutionContext context)
		=> card.Reshuffle(card.OwningPlayer, Effect);
}