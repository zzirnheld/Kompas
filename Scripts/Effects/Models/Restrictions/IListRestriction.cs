using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions
{
	/// <summary>
	/// Special, because many list restrictions have to be aware of characteristics of each other when deciding whether it's even possible to select a valid target.
	/// That is, deciding whether a valid target exists is more complicated than checking whether any one option fulfills the requirements.
	/// For instance, a "can pay costs" restriction must also take into account that you can't just pay for 4 copies of a 1 cost, if the "distinct names" requirement is also specified.
	/// </summary>
	public interface IListRestriction : IRestriction<IEnumerable<IGameCardInfo>>
	{
		public static IListRestriction SingleElement => ConstantCount(1);

		public static IListRestriction ConstantCount(int count)
		{
			var bound = new Identities.Numbers.Constant() { constant = count };
			return new ManyCards.AllOf()
			{
				elements = {
					new ManyCards.Minimum() { bound = bound },
					new ManyCards.Maximum() { bound = bound }
				}
			};
		} 

		public bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context);

		public bool IsValidClientSide(IEnumerable<IGameCardInfo> options, IResolutionContext context);

		/// <summary>
		/// If you don't specifically want to constrain the list (i.e. by deduplicating on a particular value),
		/// have this just return the source sequence.
		/// This will be overridden by restrictions like "distinct names" and "distinct costs"
		/// </summary>
		public IEnumerable<IGameCardInfo> Deduplicate(IEnumerable<IGameCardInfo> options);

		/// <summary>
        /// Get the current minimum for this resolution context.
        /// If null is passed in, the stashed minimum is returned instead.
        /// </summary>
		public int GetMinimum(IResolutionContext? context);
		/// <summary>
        /// Get the current maximum for this resolution context.
        /// If null is passed in, the stashed maximum is returned instead.
        /// </summary>
		public int GetMaximum(IResolutionContext? context);

		public void PrepareForSending(IResolutionContext context);
	}

	public static class ListRestrictionExtensions
	{
		public static readonly JsonSerializerSettings jsonSerializerSettings = new()
		{
			TypeNameHandling = TypeNameHandling.All
		};
		
		public static bool HaveEnough(this IListRestriction restriction, int currCount)
			=> restriction.GetMinimum(null) <= currCount && currCount <= restriction.GetMaximum(null);

		public static int GetStashedMinimum(this IListRestriction restriction) => restriction.GetMinimum(null);
		public static int GetStashedMaximum(this IListRestriction restriction) => restriction.GetMaximum(null);

		public static string SerializeToJSON(this IListRestriction restriction, IResolutionContext context)
		{
			restriction.PrepareForSending(context);
			return JsonConvert.SerializeObject(restriction, jsonSerializerSettings);
		}
	}
}