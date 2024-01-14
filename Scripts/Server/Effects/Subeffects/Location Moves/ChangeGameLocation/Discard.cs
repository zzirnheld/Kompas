using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate.Locations;

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
			TriggeringEventContext context = new(game: ServerGame, CardBefore: card);
			base.ChangeLocation(card);
			context.CacheCardInfoAfter();
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Vanish, context);
		}
	}
}