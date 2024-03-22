using System.Collections.Generic;
using Kompas.Effects.Models;
using Kompas.Cards.Models;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class Activation : HangingEffectSubeffect
	{
		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var tempActivation = new ActivationEffect(end: End, fallOff: FallOff,
				sourceEff: ServerEffect, resolutionContext: ResolutionContext,
				target: CardTarget, source: this);
			return new List<HangingEffect>() { tempActivation };
		}
		
		private class ActivationEffect : HangingEffect
		{
			private readonly IGameCardtarget;
			private readonly ServerSubeffect source;

			public ActivationEffect(EndCondition end, EndCondition fallOff,
				ServerEffect sourceEff, IResolutionContext resolutionContext, IGameCardtarget, ServerSubeffect source)
				: base(end, fallOff, sourceEff, resolutionContext, removeIfEnd: true)
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