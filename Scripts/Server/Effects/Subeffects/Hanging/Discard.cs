using System.Collections.Generic;
using Kompas.Effects.Models;
using Kompas.Server.Gamestate;
using Kompas.Effects.Models.Restrictions;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class Discard : HangingEffectSubeffect
	{
		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var eff = new DiscardEffect(serverGame: ServerGame,
													end: End,
													fallOff: FallOff,
											   sourceEff: Effect,
											   resolutionContext: ResolutionContext,
											   target: CardTarget);
			return new List<HangingEffect>() { eff };
		}

		/// <summary>
		/// Does nothing when created. When resolves, annihilates its target
		/// </summary>
		private class DiscardEffect : HangingEffect
		{
			private readonly GameCard target;

			public DiscardEffect(ServerGame serverGame, EndCondition end, EndCondition fallOff,
				Effect sourceEff, IResolutionContext resolutionContext, GameCard target)
				: base(serverGame, end, fallOff, sourceEff, resolutionContext, removeIfEnd: false)
			{
				this.target = target;
			}

			protected override void ResolveLogic(TriggeringEventContext context)
				=> target.Discard(sourceEff);
		}
	}
}