namespace Kompas.Effects.Models.Identities
{
	public abstract class EffectContextualLeafIdentityBase<ReturnType> : ContextualParentIdentityBase<ReturnType>
	{

		/// <summary>
		/// Gets the abstract stackable from the first one, that only knows about the context to consider,
		/// then the one that knows about both contexts if the first one came up empty.
		/// </summary>
		protected override ReturnType? AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			var contextToConsider = ContextToConsider(context, secondaryContext);
			return AbstractItemFrom(contextToConsider);
		}

		protected abstract ReturnType? AbstractItemFrom(IResolutionContext toConsider);

	}
}