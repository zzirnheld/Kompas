using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities
{

	/// <summary>
	/// An identity that needs context, either for itself or to pass on to its children.
	/// </summary>
	public abstract class ContextualParentIdentityBase<ReturnType> : ContextInitializeableBase,
		IIdentity<ReturnType>
	{
		[JsonProperty]
		public bool secondaryContext = false;

		protected IResolutionContext ContextToConsider(IResolutionContext context, IResolutionContext secondaryContext)
			=> this.secondaryContext ? secondaryContext : context;

		/// <summary>
		/// Override this one if you need to pass on BOTH contexts.
		/// </summary>
		protected abstract ReturnType AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext);

		/// <summary>
		/// Gets the abstract stackable from the first one, that only knows about the context to consider,
		/// then the one that knows about both contexts if the first one came up empty.
		/// </summary>
		public ReturnType From(IResolutionContext context, IResolutionContext secondaryContext)
		{
			ComplainIfNotInitialized();

			return AbstractItemFrom(context, secondaryContext);
		}

		public ReturnType Item => From(InitializationContext.effect.CurrentResolutionContext, default);

		protected Attack GetAttack(TriggeringEventContext effectContext)
		{
			if (effectContext.stackableEvent is Attack eventAttack) return eventAttack;
			if (effectContext.stackableCause is Attack causeAttack) return causeAttack;
			else throw new NullCardException("Stackable event wasn't an attack!");
		}
	}
}