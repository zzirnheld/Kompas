using Kompas.Effects.Models;
using Godot;
using Kompas.Effects.Models.Restrictions;

namespace Kompas.Server.Effects.Models
{
	/// <summary>
	/// Describes an effect that ends at some point.
	/// The "ending" occurs without using the stack (so it can't be negated).
	/// </summary>
	public abstract class HangingEffect
	{
		public ServerEffect Effect { get; }
		public bool RemoveIfEnd { get; }

		public EndCondition End { get; }
		public EndCondition? FallOff { get; }

		protected IResolutionContext StashedContext { get; }

		private bool ended = false;

		public readonly struct EndCondition
		{
			public ITriggerRestriction Restriction { get; init; }
			public string Condition { get; init; }
		}

		public HangingEffect(EndCondition end, EndCondition fallOff,
			ServerEffect sourceEff, IResolutionContext currentContext, bool removeIfEnd)
		{
			End = end;
			FallOff = fallOff;

			Effect = sourceEff;
			StashedContext = currentContext.Copy;
			RemoveIfEnd = removeIfEnd;
		}

		/// <summary>
		/// Determines whether the hanging effect should be canceled.
		/// If the same triggering conditions would both cause the effect to resolve and be canceled,
		/// the effect attempts to resolve.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual bool ShouldBeCanceled(TriggeringEventContext context)
			=> FallOff?.Restriction.IsValid(context, StashedContext) ?? false;

		/// <summary>
		/// Determines whether the hanging effect should go ahead and resolve
		/// </summary>
		/// <param name="context">The triggering context pertinent to this effect resolving</param>
		/// <returns>Whether the hanging effect should go ahead and resolve</returns>
		public virtual bool ShouldResolve(TriggeringEventContext context)
		{
			//if we've already ended this hanging effect, we shouldn't end it again.
			if (ended) return false;
			GD.Print($"Checking whether {this} should end for triggering context {context},"
				+ $"with saved resolution context {StashedContext}");
			return End.Restriction.IsValid(context, StashedContext);
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
			return $"{GetType()} ending when {End.Condition}";
		}
	}
}