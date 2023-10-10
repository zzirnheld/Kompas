﻿using System.Collections.Generic;
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
											   triggerRestriction: triggerRestriction,
											   endCondition: endCondition,
											   fallOffCondition: fallOffCondition,
											   sourceEff: Effect,
											   fallOffRestriction: CreateFallOffRestriction(CardTarget),
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

			public DiscardEffect(ServerGame serverGame, IRestriction<TriggeringEventContext> triggerRestriction, string endCondition,
				string fallOffCondition, IRestriction<TriggeringEventContext> fallOffRestriction,
				Effect sourceEff, IResolutionContext resolutionContext, GameCard target)
				: base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, resolutionContext, removeIfEnd: false)
			{
				this.target = target;
			}

			public override void Resolve(TriggeringEventContext context)
				=> target.Discard(sourceEff);
		}
	}
}