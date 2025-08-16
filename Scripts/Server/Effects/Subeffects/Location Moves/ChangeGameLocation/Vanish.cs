using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models.Subeffects;

public class VanishData : DiscardData { }

public class Vanish : Discard
{
	public Vanish(VanishData data) : base(data) { }

	protected override void ChangeLocation(GameCard card, IServerResolutionContext context)
	{
		var contexts = IEventContext.Build(Trigger.Vanish)
			.PrimarilyAffecting(card)
			.CausedBy(Effect)
			.Capture(() => base.ChangeLocation(card, context));
		ServerEffect.EffectsController.TriggerFor(contexts);
	}
}