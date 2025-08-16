using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DiscardData : ChangeGameLocationData { }

public class Discard : ChangeGameLocation
{
	public Discard(DiscardData data) : base(data) { }

	protected override Location Destination => Location.Discard;

	protected override void ChangeLocation(GameCard card, IServerResolutionContext context)
		=> card.Discard(Effect);
}
