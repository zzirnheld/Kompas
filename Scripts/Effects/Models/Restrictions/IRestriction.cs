namespace Kompas.Effects.Models.Restrictions
{
	public interface IRestriction<RestrictedType> : IContextInitializeable
	{
		bool IsValid(RestrictedType item, IResolutionContext context);
	}
}