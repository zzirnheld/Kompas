using KompasCore.Cards;
using Kompas.Effects.Models;
using Kompas.Server.Gamestate;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kompas.Server.Effects.Subeffects.Hanging
{
	public class Delay : HangingEffectSubeffect
	{
		public int numTimesToDelay = 0;
		public bool clearWhenResume = true;
		public override bool ContinueResolution => false;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			if (jumpIndices == null) throw new System.ArgumentNullException(nameof(jumpIndices));
		}

		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			GD.Print($"Is context null? {ResolutionContext == null}");
			GD.Print($"Are jump indices null? {jumpIndices == null}");
			var delay = new DelayEffect(game: ServerGame,
												 triggerRestriction: triggerRestriction,
												 endCondition: endCondition,
												 fallOffCondition: fallOffCondition,
												 fallOffRestriction: CreateFallOffRestriction(Source),
												 sourceEff: Effect,
												 currentContext: ResolutionContext,
												 numTimesToDelay: numTimesToDelay,
												 toResume: ServerEffect,
												 indexToResumeResolution: JumpIndex,
												 controller: EffectController,
												 targets: Effect.CardTargets,
												 cardInfoTargets: Effect.CardInfoTargets,
												 spaces: Effect.SpaceTargets,
												 clearIfResolve: clearWhenResume);
			return new List<HangingEffect>() { delay };
		}

		private class DelayEffect : HangingEffect
		{
			private readonly int numTimesToDelay;
			private int numTimesDelayed;
			private readonly ServerEffect toResume;
			private readonly int indexToResumeResolution;
			private readonly ServerPlayer controller;
			private readonly List<GameCard> targets;
			private readonly List<GameCardInfo> cardInfoTargets;
			private readonly List<Space> spaces;

			public DelayEffect(ServerGame game, IRestriction<TriggeringEventContext> triggerRestriction, string endCondition,
				string fallOffCondition, IRestriction<TriggeringEventContext> fallOffRestriction, Effect sourceEff, IResolutionContext currentContext,
				int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution, ServerPlayer controller,
				IEnumerable<GameCard> targets, IEnumerable<GameCardInfo> cardInfoTargets, IEnumerable<Space> spaces,
				bool clearIfResolve)
				: base(game, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, clearIfResolve)
			{
				this.numTimesToDelay = numTimesToDelay;
				this.toResume = toResume;
				this.indexToResumeResolution = indexToResumeResolution;
				this.controller = controller;
				GD.Print($"Targets are {string.Join(",", targets?.Select(c => c.ToString()) ?? new string[] { "Null" })}");
				this.targets = new List<GameCard>(targets);
				this.cardInfoTargets = new List<GameCardInfo>(cardInfoTargets);
				this.spaces = new List<Space>(spaces);
				numTimesDelayed = 0;
			}

			public override bool ShouldResolve(TriggeringEventContext context)
			{
				Godot.GD.Print($"Checking if delayed hanging effect should end for context {context}, {numTimesDelayed}/{numTimesToDelay}");
				//first check any other logic
				if (!base.ShouldResolve(context)) return false;

				//if it should otherwise be fine, but we haven't waited enough times, delay further
				if (numTimesDelayed < numTimesToDelay)
				{
					numTimesDelayed++;
					return false;
				}
				else
				{
					numTimesDelayed = 0;
					return true;
				}
			}

			public override void Resolve(TriggeringEventContext context)
			{
				var myContext = new ResolutionContext(context, indexToResumeResolution, targets, default, cardInfoTargets, spaces, default, default, default);
				serverGame.effectsController.PushToStack(toResume, controller, myContext);
			}
		}
	}
}