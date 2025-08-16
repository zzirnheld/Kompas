using System.Collections.Generic;
using Kompas.Effects.Models;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

public class DiscardData : HangingEffectData { }

public class Discard : HangingEffectSubeffect<DiscardData>
{
	public Discard(DiscardData data) : base(data) { }

	protected override IEnumerable<HangingEffect> CreateHangingEffects(IServerResolutionContext context)
	{
		var eff = new DiscardEffect(end: End, fallOff: FallOff,
			sourceEff: ServerEffect, resolutionContext: context,
			target: GetCardTarget(context));
		return new List<HangingEffect>() { eff };
	}

	/// <summary>
	/// Does nothing when created. When resolves, annihilates its target
	/// </summary>
	private class DiscardEffect : HangingEffect
	{
		private readonly GameCard target;

		public DiscardEffect(EndCondition end, EndCondition fallOff,
			ServerEffect sourceEff, IResolutionContext resolutionContext, GameCard target)
			: base(end, fallOff, sourceEff, resolutionContext, removeIfEnd: false)
		{
			this.target = target;
		}

		protected override void ResolveLogic(IEventContext context)
			=> target.Discard(Effect);
	}
}