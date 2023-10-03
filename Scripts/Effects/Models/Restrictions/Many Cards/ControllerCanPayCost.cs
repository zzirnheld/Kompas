using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public class ControllerCanPayCost : ListRestrictionElementBase
	{
		protected override bool IsValidLogic(IEnumerable<IGameCard> item, IResolutionContext context)
			=> item.Select(c => c.Cost).Sum() <= InitializationContext.Controller.Pips;

		public override bool AllowsValidChoice(IEnumerable<IGameCard> options, IResolutionContext context)
		{
			if (!(InitializationContext.parent is IListRestriction parent)) return true;

			//Accounts for all deduplicating of other possible things like distinct name, but doesn't check that there are enough (those deduplicators check that)
			return parent.Deduplicate(options)
				.Select(c => c.Cost)
				.OrderBy(c => c)
				.Take(parent.GetMinimum(context))
				.Sum() <= InitializationContext.Controller.Pips;
		}
	}
}