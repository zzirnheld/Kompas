using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PlayData : ChangeGameLocationData { }

public class Play : ChangeGameLocation
{
	public Play(PlayData data) : base(data) { }

	protected override Location Destination => Location.Board;

	protected override void ChangeLocation(GameCard card, IServerResolutionContext context)
		=> card.Play(GetSpaceTarget(context), GetPlayerTarget(context), Effect);
}