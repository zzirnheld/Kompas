using Kompas.Effects.Models;
using Kompas.Server.Gamestate;
using Godot;
using Kompas.Effects.Models.Restrictions;

namespace Kompas.Server.Effects.Models
{
	public abstract class HangingEffect
	{
		public readonly Effect sourceEff;
		public readonly string endCondition;
		public readonly string fallOffCondition;
		public readonly IRestriction<TriggeringEventContext> fallOffRestriction;

		public bool RemoveIfEnd { get; }

		private bool ended = false;
		private readonly IRestriction<TriggeringEventContext> triggerRestriction;
		protected readonly ServerGame serverGame;
		private readonly IResolutionContext savedContext;

		public HangingEffect(ServerGame serverGame, IRestriction<TriggeringEventContext> triggerRestriction, string endCondition,
			string fallOffCondition, IRestriction<TriggeringEventContext> fallOffRestriction,
			Effect sourceEff, IResolutionContext currentContext, bool removeIfEnd)
		{
			this.serverGame = serverGame != null ? serverGame : throw new System.ArgumentNullException(nameof(serverGame), "ServerGame in HangingEffect must not be null");
			this.triggerRestriction = triggerRestriction ?? throw new System.ArgumentNullException(nameof(triggerRestriction), "Trigger Restriction in HangingEffect must not be null");
			this.endCondition = endCondition;

			this.fallOffCondition = fallOffCondition;
			this.fallOffRestriction = fallOffRestriction;

			this.sourceEff = sourceEff;
			savedContext = currentContext.Copy;
			RemoveIfEnd = removeIfEnd;
		}

		public virtual bool ShouldBeCanceled(TriggeringEventContext context)
			=> fallOffRestriction.IsValid(context, IResolutionContext.NotResolving);

		public virtual bool ShouldResolve(TriggeringEventContext context)
		{
			//if we've already ended this hanging effect, we shouldn't end it again.
			if (ended) return false;
			GD.Print($"Checking whether {this} should end for context {context}, with saved context {savedContext}");
			return triggerRestriction.IsValid(context, savedContext);
		}

		/// <summary>
		/// Resolves the hanging effect.
		/// This usually amounts to canceling whatever effect the hanging effect originally applied,
		/// or maybe resuming a delayed effect.
		/// </summary>
		/// <param name="context">The context in which the effect is being resolved.</param>
		public abstract void Resolve(TriggeringEventContext context);

		public override string ToString()
		{
			return $"{GetType()} ending when {endCondition}";
		}
	}
}