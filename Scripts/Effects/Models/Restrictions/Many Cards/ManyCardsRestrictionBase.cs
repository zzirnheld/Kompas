using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public abstract class ListRestrictionElementBase : RestrictionBase<IEnumerable<GameCardBase>>, IListRestriction
	{
		public abstract bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context);
		public virtual bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context) => IsValid(options, context);
		public virtual IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options) => options; //No dedup, by default
		public virtual int GetMinimum(IResolutionContext context) => 0;
		public virtual int GetMaximum(IResolutionContext context) => int.MaxValue;

		public virtual void PrepareForSending(IResolutionContext context) { }
	}
}