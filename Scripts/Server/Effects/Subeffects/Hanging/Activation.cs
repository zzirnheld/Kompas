using System.Collections.Generic;
using Kompas.Effects.Models;
using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

public class ActivationData : HangingEffectData { }

public class Activation : HangingEffectSubeffect<ActivationData>
{
	public Activation(ActivationData data) : base(data) { }

	protected override IEnumerable<HangingEffect> CreateHangingEffects(IServerResolutionContext context)
	{
		var tempActivation = new ActivationEffect(end: End, fallOff: FallOff,
			sourceEff: ServerEffect, resolutionContext: context,
			target: GetCardTarget(context), source: this);
		return new List<HangingEffect>() { tempActivation };
	}

	private class ActivationEffect : HangingEffect
	{
		private readonly GameCard target;
		private readonly IServerSubeffect source;

		public ActivationEffect(EndCondition end, EndCondition fallOff,
			ServerEffect sourceEff, IResolutionContext resolutionContext, GameCard target, IServerSubeffect source)
			: base(end, fallOff, sourceEff, resolutionContext, removeIfEnd: true)
		{
			this.target = target ?? throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging activation");
			this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging activation effect from no subeffect");
			target.SetActivated(true, source.ServerEffect);
		}

		protected override void ResolveLogic(IEventContext context)
			=> target.SetActivated(false, source.ServerEffect);
	}
}