using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IActivationRestriction : IRestriction<IPlayer>, IAllOf<IPlayer>
	{
	}

	public static class IActivationRestrictionExtensions
	{
		public static bool IsPotentiallyValidActivation(this IActivationRestriction restriction, IPlayer activator)
			=> restriction.IsValidIgnoring(activator, IResolutionContext.NotResolving,
				restriction => restriction is not Gamestate.NothingHappening);
	}
}