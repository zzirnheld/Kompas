using System.Collections.Generic;

namespace Kompas.Effects.Models.Identities
{
	public abstract class ContextlessLeafIdentityBase<ReturnType> : ContextInitializeableBase,
		IIdentity<ReturnType>, IIdentity<IReadOnlyCollection<ReturnType>>
	{
		protected abstract ReturnType? AbstractItem { get; }

		public ReturnType? Item
		{
			get
			{
				ComplainIfNotInitialized();
				return AbstractItem;
			}
		}

		public ReturnType? From(IResolutionContext? context, IResolutionContext? secondaryContext) => Item;

		IReadOnlyCollection<ReturnType>? IIdentity<IReadOnlyCollection<ReturnType>>.From(IResolutionContext context, IResolutionContext secondaryContext)
		{
			ReturnType? item = From(context, secondaryContext);
			return item == null
				? default
				: new ReturnType[] { item };
		}
	}
}