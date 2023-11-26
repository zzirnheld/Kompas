using Kompas.Effects.Models;
using Kompas.Server.Gamestate;
using Godot;
using Kompas.Effects.Models.Restrictions;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Triggering;
using Kompas.Effects.Models.Restrictions.Gamestate;

namespace Kompas.Server.Effects.Models
{
	public abstract class HangingEffect
	{
		public readonly Effect sourceEff;
		public bool RemoveIfEnd { get; }

		public EndCondition end;
		public EndCondition? fallOff;

		private bool ended = false;
		protected readonly ServerGame serverGame;
		protected IResolutionContext StashedContext { get; }

		public readonly struct EndCondition
		{
			public IRestriction<TriggeringEventContext> Restriction { get; init; }
			public string Condition { get; init; }
		}

		public HangingEffect(ServerGame serverGame, EndCondition end, EndCondition fallOff,
			Effect sourceEff, IResolutionContext currentContext, bool removeIfEnd)
		{
			this.serverGame = serverGame
				?? throw new System.ArgumentNullException(nameof(serverGame), "ServerGame in HangingEffect must not be null");
			this.end = end;
			this.fallOff = fallOff;

			this.sourceEff = sourceEff;
			StashedContext = currentContext.Copy;
			RemoveIfEnd = removeIfEnd;
		}

		public virtual bool ShouldBeCanceled(TriggeringEventContext context)
			=> fallOff?.Restriction.IsValid(context, IResolutionContext.NotResolving) ?? false;

		public virtual bool ShouldResolve(TriggeringEventContext context)
		{
			//if we've already ended this hanging effect, we shouldn't end it again.
			if (ended) return false;
			GD.Print($"Checking whether {this} should end for context {context}, with saved context {StashedContext}");
			return end.Restriction.IsValid(context, StashedContext);
		}

		/// <summary>
		/// Resolves the hanging effect.
		/// This usually amounts to canceling whatever effect the hanging effect originally applied,
		/// or maybe resuming a delayed effect.
		/// </summary>
		/// <param name="context">The context in which the effect is being resolved.</param>
		public void Resolve(TriggeringEventContext context)
		{
			ended = true;
			ResolveLogic(context);
		}

		protected abstract void ResolveLogic(TriggeringEventContext context);

		public override string ToString()
		{
			return $"{GetType()} ending when {end.Condition}";
		}
	}
}