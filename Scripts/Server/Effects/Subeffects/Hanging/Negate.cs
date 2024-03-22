using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class Negate : HangingEffectSubeffect
	{
		public bool negated = true;

		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var tempNegation = new NegationEffect(end: End, fallOff: FallOff,
				source: ServerEffect, currentContext: ResolutionContext,
				target: CardTarget, negated: negated);
			return new List<HangingEffect>() { tempNegation };
		}

		public class NegationEffect : HangingEffect
		{
			private readonly IGameCardtarget;
			private readonly bool negated;

			public NegationEffect(EndCondition end, EndCondition fallOff,
				ServerEffect source, IResolutionContext currentContext,
				GameCard target, bool negated)
				: base(end, fallOff, source, currentContext, removeIfEnd: false)
			{
				this.target = target;
				this.negated = negated;
				target.SetNegated(negated, source);
			}

			protected override void ResolveLogic(TriggeringEventContext context) => target.SetNegated(!negated, Effect);
		}
	}
}