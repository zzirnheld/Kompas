using Kompas.Effects.Models;
using System.Collections.Generic;
using Godot;
using Kompas.Server.Gamestate.Players;
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
			if (jumpIndices == null) throw new System.InvalidOperationException(nameof(jumpIndices));
		}

		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			Logger.Log($"Is context null? {ResolutionContext == null}");
			Logger.Log($"Are jump indices null? {jumpIndices == null}");
			var context = ResolutionContext ?? throw new EffectNotResolvingException(Effect);
			var controller = ServerEffect.CurrentServerResolutionContext?.ControllingPlayer
				?? throw new InvalidOperationException();
			var delay = new DelayEffect(end: End, fallOff: FallOff,
				sourceEff: ServerEffect, currentContext: context,
				numTimesToDelay: numTimesToDelay, indexToResumeResolution: JumpIndex,
				controller: controller, clearIfResolve: clearWhenResume);
			return new List<HangingEffect>() { delay };
		}

		private class DelayEffect : HangingEffect
		{
			private readonly int numTimesToDelay;
			private int numTimesDelayed;
			private readonly int indexToResumeResolution;
			private readonly ServerPlayer controller;

			public DelayEffect(EndCondition end, EndCondition fallOff,
				ServerEffect sourceEff, IResolutionContext currentContext,
				int numTimesToDelay, int indexToResumeResolution,
				ServerPlayer controller, bool clearIfResolve)
				: base(end, fallOff, sourceEff, currentContext, clearIfResolve)
			{
				this.numTimesToDelay = numTimesToDelay;
				this.indexToResumeResolution = indexToResumeResolution;
				this.controller = controller;
				numTimesDelayed = 0;
			}

			public override bool ShouldResolve(TriggeringEventContext context)
			{
				Logger.Log($"Checking if delayed hanging effect should end for context {context}, {numTimesDelayed}/{numTimesToDelay}");
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
				var myContext = ServerResolutionContext.Resume(StashedContext,
					context, controller, indexToResumeResolution);
				Effect.ServerGame.StackController.PushToStack(Effect, controller, myContext);
			}
		}
	}
}