using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public abstract class ListRestrictionElementBase : RestrictionBase<IEnumerable<IGameCard>>, IListRestriction
	{
		public abstract bool AllowsValidChoice(IEnumerable<IGameCard> options, IResolutionContext context);
		public virtual bool IsValidClientSide(IEnumerable<IGameCard> options, IResolutionContext context) => IsValid(options, context);
		public virtual IEnumerable<IGameCard> Deduplicate(IEnumerable<IGameCard> options) => options; //No dedup, by default
		public virtual int GetMinimum(IResolutionContext context) => 0;
		public virtual int GetMaximum(IResolutionContext context) => int.MaxValue;

		public virtual void PrepareForSending(IResolutionContext context) { }
	}
}