using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public abstract class Distinct : ListRestrictionElementBase
	{
		protected delegate object DistinguishingValueSelector(IGameCard card);
		protected abstract DistinguishingValueSelector SelectDistinguishingValue { get; }

		private class DistinctCardComparer : IEqualityComparer<IGameCard>
		{
			private readonly DistinguishingValueSelector selectDistinguishingValue;

			public bool Equals(IGameCard x, IGameCard y) => selectDistinguishingValue(x) == selectDistinguishingValue(y);
			public int GetHashCode(IGameCard obj) => selectDistinguishingValue(obj).GetHashCode();

			public DistinctCardComparer(DistinguishingValueSelector selectDistinguishingValue)
			{
				this.selectDistinguishingValue = selectDistinguishingValue;
			}
		}

		//Ensure there exists a selection that fits the required minimum count
		public override bool AllowsValidChoice(IEnumerable<IGameCard> options, IResolutionContext context)
		{
			if (!(InitializationContext.parent is IListRestriction parent)) return true;

			return parent.Deduplicate(options)
				.Count() >= parent.GetMinimum(context);
		}

		//Ensure that particular selection is distinct
		protected override bool IsValidLogic(IEnumerable<IGameCard> options, IResolutionContext context)
			=> options.Count()
			== options.Distinct(new DistinctCardComparer(SelectDistinguishingValue))
						.Count();

		public override IEnumerable<IGameCard> Deduplicate(IEnumerable<IGameCard> options)
			=> options.Distinct(new DistinctCardComparer(SelectDistinguishingValue));
	}

	public class DistinctNames : Distinct
	{
		protected override DistinguishingValueSelector SelectDistinguishingValue => card => card.CardName;
	}

	public class DistinctCosts : Distinct
	{
		protected override DistinguishingValueSelector SelectDistinguishingValue => card => card.Cost;
	}
}