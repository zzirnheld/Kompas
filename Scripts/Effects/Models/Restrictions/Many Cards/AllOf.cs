using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public class AllOf : AllOfBase<IEnumerable<GameCardBase>, IListRestriction>, IListRestriction
	{
		private bool clientSide;

		protected override bool Validate(IListRestriction element, IEnumerable<GameCardBase> item, IResolutionContext context)
			=> clientSide
			? element.IsValidClientSide(item, context)
			: element.IsValid(item, context);

		public bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
		{
			ComplainIfNotInitialized();
			return GetMinimum(context) <= GetMaximum(context)	//There exists a number of cards that both the min and max permit
				&& GetMinimum(context) <= options.Count() 		//Enough options to hit the minimum
				&& GetMaximum(context) >= 0						//Allowed to select a nonnegative amount of cards
				&& elements.All(elem => elem.AllowsValidChoice(options, context));
		}

		public bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context)
		{
			clientSide = true;
			bool ret = IsValid(options, context);
			clientSide = false;
			return ret;
		}

		public IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options)
		{
			var localOptions = options;
			foreach (var elem in elements) localOptions = elem.Deduplicate(localOptions);
			return localOptions;
		}

		public int GetMinimum(IResolutionContext context)
			=> elements
				.Select(elem => elem.GetMinimum(context))
				.DefaultIfEmpty(0)
				.Max();

		public int GetMaximum(IResolutionContext context)
			=> elements
				.Select(elem => elem.GetMaximum(context))
				.DefaultIfEmpty(int.MaxValue)
				.Min();

		public void PrepareForSending(IResolutionContext context)
		{
			foreach (var elem in elements) elem.PrepareForSending(context);
		}
	}
}