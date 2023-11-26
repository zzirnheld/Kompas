using System.Collections.Generic;
using Kompas.Effects.Models;
using Kompas.Server.Gamestate;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class Activation : HangingEffectSubeffect
	{
		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var tempActivation = new ActivationEffect(serverGame: ServerGame,
													end: End,
													fallOff: FallOff,
													sourceEff: Effect,
													resolutionContext: ResolutionContext,
													target: CardTarget,
													source: this);
			return new List<HangingEffect>() { tempActivation };
		}
		
		private class ActivationEffect : HangingEffect
		{
			private readonly GameCard target;
			private readonly ServerSubeffect source;

			public ActivationEffect(ServerGame serverGame, EndCondition end, EndCondition fallOff,
				Effect sourceEff, IResolutionContext resolutionContext, GameCard target, ServerSubeffect source)
				: base(serverGame, end, fallOff, sourceEff, resolutionContext, removeIfEnd: true)
			{
				this.target = target ?? throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging activation");
				this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging activation effect from no subeffect");
				target.SetActivated(true, source.ServerEffect);
			}

			protected override void ResolveLogic(TriggeringEventContext context)
				=> target.SetActivated(false, source.ServerEffect);
		}
	}
}