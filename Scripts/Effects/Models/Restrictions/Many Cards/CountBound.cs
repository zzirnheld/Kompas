using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.ManyCards
{
	public abstract class CountBound : ListRestrictionElementBase
	{
		#nullable disable
		/// <summary>
		/// The bound, as defined by the actual card json.
		/// Might have to reference stuff about the current context, like the current effect X value.
		/// </summary>
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> bound;
		#nullable restore

		/// <summary>
		/// Used for sending a current minimum to the client.
		/// Obviously, the simpler solution is to assume that we're always either a constant or X,
		/// but that makes any other manipulation much harder than it needs to be.
		/// This is more flexible long-term, even if it is more annoying.
		/// </summary>
		[JsonProperty]
		public int stashedBound;

		public override void PrepareForSending(IResolutionContext context) => stashedBound = bound.From(context);


		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);

			bound.Initialize(initializationContext);
		}
	}

	public class Minimum : CountBound
	{
		protected override bool IsValidLogic(IEnumerable<IGameCardInfo>? item, IResolutionContext context)
			=> item?.Count() >= bound.From(context);

		public override bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context)
			=> options.Count() >= bound.From(context);

		public override bool IsValidClientSide (IEnumerable<IGameCardInfo>? options, IResolutionContext context)
			=> options?.Count() >= stashedBound;

		public override int GetMinimum(IResolutionContext? context)
			=> context == null
				? stashedBound
				: bound.From(context);
	}

	public class Maximum : CountBound
	{
		protected override bool IsValidLogic(IEnumerable<IGameCardInfo>? item, IResolutionContext context)
			=> item?.Count() <= bound.From(context);

		public override bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context)
			=> true;

		public override bool IsValidClientSide (IEnumerable<IGameCardInfo>? options, IResolutionContext context)
			=> options?.Count() <= stashedBound;

		public override int GetMaximum(IResolutionContext? context)
			=> context == null
				? stashedBound
				: bound.From(context);
	}
}