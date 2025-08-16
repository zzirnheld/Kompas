using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class HandData : ChangeGameLocationData { }

public class Hand : ChangeGameLocation
{
	public Hand(HandData data) : base(data) { }

	protected override Location Destination => Location.Hand;

	protected override void ChangeLocation(GameCard card, IServerResolutionContext context)
		=> card.Hand(card.OwningPlayer, Effect);
}