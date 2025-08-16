using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AnnihilateData : ChangeGameLocationData { }

public class Annihilate : ChangeGameLocation<AnnihilateData>
{
	public Annihilate(AnnihilateData data) : base(data) { }

	protected override Location Destination => Location.Annihilation;

	protected override void ChangeLocation(GameCard card, IServerResolutionContext context)
		=> card.Annihilate(Effect);
}