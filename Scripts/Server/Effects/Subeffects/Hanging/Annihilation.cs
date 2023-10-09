using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class Annihilation : HangingEffectSubeffect
	{
		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var eff = new AnnihilationEffect(serverGame: ServerGame,
													triggerRestriction: triggerRestriction,
													endCondition: endCondition,
													fallOffCondition: fallOffCondition,
													fallOffRestriction: CreateFallOffRestriction(CardTarget),
													sourceEff: Effect,
													resolutionContext: ResolutionContext,
													target: CardTarget);
			return new List<HangingEffect>() { eff };
		}

		/// <summary>
		/// Does nothing when created. When resolves, annihilates its target
		/// </summary>
		private class AnnihilationEffect : HangingEffect
		{
			private readonly GameCard target;

			public AnnihilationEffect(ServerGame serverGame, IRestriction<TriggeringEventContext> triggerRestriction, string endCondition,
				string fallOffCondition, IRestriction<TriggeringEventContext> fallOffRestriction,
				Effect sourceEff, IResolutionContext resolutionContext, GameCard target)
				: base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, resolutionContext, removeIfEnd: true)
			{
				this.target = target;
			}

			public override void Resolve(TriggeringEventContext context) => target.Annihilate(sourceEff);

			public override string ToString()
			{
				return $"{base.ToString()} affecting {target}";
			}
		}
	}
}