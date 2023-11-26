namespace Kompas.Effects.Models.Identities
{
	/// <summary>
	/// Can be used whether or not the caller does or doesn't care about an ActivationContext)
	/// </summary>
	public interface IIdentity<ReturnType> : IContextInitializeable
	{
		public ReturnType? From(IResolutionContext? context, IResolutionContext? secondaryContext);
	}

	public static class IdentityExtensions
	{
		public static ReturnType? From<ReturnType>(this IIdentity<ReturnType> identity,
			TriggeringEventContext? triggeringContext, IResolutionContext? resolutionContext)
				=> identity.From(IResolutionContext.Dummy(triggeringContext), resolutionContext);

		public static ReturnType? From<ReturnType>(this IIdentity<ReturnType> identity,
			IResolutionContext? resolutionContext)
				=> identity.From(resolutionContext, IResolutionContext.NotResolving);
	}
}