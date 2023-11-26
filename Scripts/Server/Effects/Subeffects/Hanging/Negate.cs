using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class Negate : HangingEffectSubeffect
	{
		public bool negated = true;

		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var tempNegation = new NegationEffect(serverGame: ServerGame,
													end: End,
													fallOff: FallOff,
														 currentContext: ResolutionContext,
														 target: CardTarget,
														 source: this,
														 negated: negated);
			return new List<HangingEffect>() { tempNegation };
		}

		public class NegationEffect : HangingEffect
		{
			private readonly GameCard target;
			private readonly ServerSubeffect source;
			private readonly bool negated;

			public NegationEffect(ServerGame serverGame, EndCondition end, EndCondition fallOff,
				IResolutionContext currentContext, GameCard target, ServerSubeffect source, bool negated)
				: base(serverGame, end, fallOff, source.Effect, currentContext, removeIfEnd: false)
			{
				this.target = target ?? throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging negation");
				this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging negation effect from no subeffect");
				this.negated = negated;
				target.SetNegated(negated, source.Effect);
			}

			protected override void ResolveLogic(TriggeringEventContext context) => target.SetNegated(!negated, source.Effect);
		}
	}
}