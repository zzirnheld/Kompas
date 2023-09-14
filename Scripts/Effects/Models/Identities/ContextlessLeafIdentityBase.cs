namespace Kompas.Effects.Models.Identities
{
	public abstract class ContextlessLeafIdentityBase<ReturnType> : ContextInitializeableBase,
		IIdentity<ReturnType>
	{
		protected abstract ReturnType AbstractItem { get; }

		public ReturnType Item
		{
			get
			{
				ComplainIfNotInitialized();
				return AbstractItem;
			}
		}

		public ReturnType From(IResolutionContext context, IResolutionContext secondaryContext) => Item;
	}
}