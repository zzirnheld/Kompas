using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public abstract class Distinct : ListRestrictionElementBase
	{
		protected delegate object? DistinguishingValueSelector(IGameCardInfo? card);
		protected abstract DistinguishingValueSelector SelectDistinguishingValue { get; }

		private class DistinctCardComparer : IEqualityComparer<IGameCardInfo>
		{
			private readonly DistinguishingValueSelector selectDistinguishingValue;

			public bool Equals(IGameCardInfo? x, IGameCardInfo? y) => selectDistinguishingValue(x) == selectDistinguishingValue(y);
			public int GetHashCode(IGameCardInfo obj) => selectDistinguishingValue(obj)?.GetHashCode() ?? 0;

			public DistinctCardComparer(DistinguishingValueSelector selectDistinguishingValue)
			{
				this.selectDistinguishingValue = selectDistinguishingValue;
			}
		}

		//Ensure there exists a selection that fits the required minimum count
		public override bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context)
		{
			if (InitializationContext.parent is not IListRestriction parent) return true;

			return parent.Deduplicate(options)
				.Count() >= parent.GetMinimum(context);
		}

		//Ensure that particular selection is distinct
		protected override bool IsValidLogic(IEnumerable<IGameCardInfo>? options, IResolutionContext context)
			=> options?.Count()
			== options?.Distinct(new DistinctCardComparer(SelectDistinguishingValue))
						.Count();

		public override IEnumerable<IGameCardInfo> Deduplicate(IEnumerable<IGameCardInfo> options)
			=> options.Distinct(new DistinctCardComparer(SelectDistinguishingValue));
	}

	public class DistinctNames : Distinct
	{
		protected override DistinguishingValueSelector SelectDistinguishingValue => card => card?.CardName;
	}

	public class DistinctCosts : Distinct
	{
		protected override DistinguishingValueSelector SelectDistinguishingValue => card => card?.Cost;
	}
}