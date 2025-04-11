using Kompas.Effects.Models;
using System.Collections.Generic;
using Kompas.Server.Gamestate.Players;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

public class Delay : HangingEffectSubeffect
{
	public int numTimesToDelay = 0;
	public string? blurbAfterDelay;
	public bool clearWhenResume = true;

	public override bool ContinueResolution => false;
	private string BlurbAfterDelay => blurbAfterDelay ?? Effect.InitialBlurb;

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		if (jumpIndices == null) throw new System.InvalidOperationException(nameof(jumpIndices));
	}

	protected override IEnumerable<HangingEffect> CreateHangingEffects(IServerResolutionContext context)
	{
		Logger.Log($"Are jump indices null? {jumpIndices == null}");
		var controller = context.ControllingPlayer
			?? throw new System.InvalidOperationException();
		var delay = new DelayEffect(end: End, fallOff: FallOff,
			sourceEff: ServerEffect, currentContext: context,
			numTimesToDelay: numTimesToDelay, indexToResumeResolution: JumpIndex,
			controller: controller, clearIfResolve: clearWhenResume, blurb: BlurbAfterDelay);
		return new List<HangingEffect>() { delay };
	}

	private class DelayEffect : HangingEffect
	{
		private readonly int numTimesToDelay;
		private int numTimesDelayed;
		private readonly int indexToResumeResolution;
		private readonly ServerPlayer controller;
		private readonly string blurb;

		public DelayEffect(EndCondition end, EndCondition fallOff,
			ServerEffect sourceEff, IResolutionContext currentContext,
			int numTimesToDelay, int indexToResumeResolution,
			ServerPlayer controller, bool clearIfResolve, string blurb)
			: base(end, fallOff, sourceEff, currentContext, clearIfResolve)
		{
			this.numTimesToDelay = numTimesToDelay;
			this.indexToResumeResolution = indexToResumeResolution;
			this.controller = controller;
			this.blurb = blurb;
			
			numTimesDelayed = 0;
		}

		public override bool ShouldResolve(IEventContext context)
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

		protected override void ResolveLogic(IEventContext context)
		{
			var myContext = ServerResolutionContext.Resume(StashedContext,
				context, controller, indexToResumeResolution, blurb);
			var resolution = new ServerEffectResolution(Effect, myContext);
			Effect.ServerGame.StackController.PushToStack(resolution);
		}
	}
}