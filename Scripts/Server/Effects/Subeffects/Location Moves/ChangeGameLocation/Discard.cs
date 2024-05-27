using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;
using Kompas.Server.Effects.Controllers;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Discard : ChangeGameLocation
	{
		protected override Location Destination => Location.Discard;

		protected override void ChangeLocation(GameCard card) => card.Discard(Effect);
	}

	public class Vanish : Discard
	{
		protected override void ChangeLocation(GameCard card)
		{
			var contexts = IEventContext.Build(Trigger.Vanish)
				.PrimarilyAffecting(card)
				.CausedBy(Effect)
				.Capture(() => base.ChangeLocation(card));
			ServerEffect.EffectsController.TriggerFor(contexts);
		}
	}
}