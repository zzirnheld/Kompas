using System.Collections.Generic;
using KompasCore.Cards;
using Kompas.Effects.Models;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Effects.Subeffects.Hanging
{
	public class Activation : HangingEffectSubeffect
	{
		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			var tempActivation = new ActivationEffect(serverGame: ServerGame,
													triggerRestriction: triggerRestriction,
													endCondition: endCondition,
													fallOffCondition: fallOffCondition,
													fallOffRestriction: CreateFallOffRestriction(CardTarget),
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

			public ActivationEffect(ServerGame serverGame, IRestriction<TriggeringEventContext> triggerRestriction, string endCondition,
				string fallOffCondition, IRestriction<TriggeringEventContext> fallOffRestriction,
				Effect sourceEff, IResolutionContext resolutionContext, GameCard target, ServerSubeffect source)
				: base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, resolutionContext, removeIfEnd: true)
			{
				this.target = target != null ? target : throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging activation");
				this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging activation effect from no subeffect");
				target.SetActivated(true, source.ServerEffect);
			}

			public override void Resolve(TriggeringEventContext context)
				=> target.SetActivated(false, source.ServerEffect);
		}
	}
}