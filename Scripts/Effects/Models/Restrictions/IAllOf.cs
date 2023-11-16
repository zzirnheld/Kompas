using System.Collections.Generic;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IAllOf<RestrictedType> : IRestriction<RestrictedType>
	{
		public delegate bool ShouldIgnore(IRestriction<RestrictedType> restriction);
		public bool IsValidIgnoring(RestrictedType? item, IResolutionContext? context, ShouldIgnore ignorePredicate);

		public IEnumerable<IRestriction<RestrictedType>> GetElements();
	}
}