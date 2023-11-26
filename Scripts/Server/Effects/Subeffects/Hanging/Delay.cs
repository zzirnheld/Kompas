using Kompas.Effects.Models;
using Kompas.Server.Gamestate;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Server.Gamestate.Players;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Effects.Models.Restrictions;
using System;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
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
			var context = ResolutionContext ?? throw new EffectNotResolvingException(Effect);
			var controller = ServerEffect.CurrentServerResolutionContext?.ControllingPlayer
				?? throw new InvalidOperationException();
			var delay = new DelayEffect(game: ServerGame,
												end: End,
												fallOff: FallOff,
												 sourceEff: Effect,
												 currentContext: context,
												 numTimesToDelay: numTimesToDelay,
												 toResume: ServerEffect,
												 indexToResumeResolution: JumpIndex,
												 controller: controller,
												 resolutionContext: ResolutionContext,
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
			private readonly IResolutionContext stashedResolutionContext;

			public DelayEffect(ServerGame game, EndCondition end, EndCondition fallOff,
				Effect sourceEff, IResolutionContext currentContext,
				int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution, ServerPlayer controller,
				IResolutionContext resolutionContext, bool clearIfResolve)
				: base(game, end, fallOff, sourceEff, currentContext, clearIfResolve)
			{
				this.numTimesToDelay = numTimesToDelay;
				this.toResume = toResume;
				this.indexToResumeResolution = indexToResumeResolution;
				this.controller = controller;
				stashedResolutionContext = resolutionContext.Copy;
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

			protected override void ResolveLogic(TriggeringEventContext context)
			{
				var myContext = ServerResolutionContext.Resume(stashedResolutionContext,
					context, controller, indexToResumeResolution);
				serverGame.StackController.PushToStack(toResume, controller, myContext);
			}
		}
	}
}