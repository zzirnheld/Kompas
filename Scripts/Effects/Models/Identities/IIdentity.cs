namespace Kompas.Effects.Models.Identities
{
	/// <summary>
	/// Can be used whether or not the caller does or doesn't care about an ActivationContext)
	/// </summary>
	public interface IIdentity<ReturnType> : IContextInitializeable
	{
		//TODO: figure out a better way to return a collection, knowing it's not null, because collections shouldn't be null.
		//for that matter, a lot of stuff shouldn't be null. a further pass could try and identify exactly where I'm using null semantically,
		//and try and replace that
		public ReturnType? From(IResolutionContext context, IResolutionContext secondaryContext);
	}

	public static class IdentityExtensions
	{
		public static ReturnType? From<ReturnType>(this IIdentity<ReturnType> identity,
			TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
				=> identity.From(IResolutionContext.Dummy(triggeringContext), resolutionContext);

		public static ReturnType? From<ReturnType>(this IIdentity<ReturnType> identity,
			IResolutionContext resolutionContext)
				=> identity.From(resolutionContext, IResolutionContext.NotResolving);
	}
}