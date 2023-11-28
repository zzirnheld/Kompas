using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
    public class Annihilation : HangingEffectSubeffect
	{
		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var eff = new AnnihilationEffect(end: End, fallOff: FallOff,
				sourceEff: ServerEffect, resolutionContext: ResolutionContext,
				target: CardTarget);
			return new List<HangingEffect>() { eff };
		}

		/// <summary>
		/// Does nothing when created. When resolves, annihilates its target
		/// </summary>
		private class AnnihilationEffect : HangingEffect
		{
			private readonly GameCard target;

			public AnnihilationEffect(EndCondition end, EndCondition fallOff,
				ServerEffect sourceEff, IResolutionContext resolutionContext, GameCard target)
				: base(end, fallOff, sourceEff, resolutionContext, removeIfEnd: true)
			{
				this.target = target;
			}

			protected override void ResolveLogic(TriggeringEventContext context) => target.Annihilate(sourceEff);

			public override string ToString()
			{
				return $"{base.ToString()} affecting {target}";
			}
		}
	}
}