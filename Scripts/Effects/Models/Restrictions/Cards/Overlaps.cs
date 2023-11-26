using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Overlaps : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCardInfo> other;
		/// <summary>
		/// An optional override to check if the card to be tested overlaps another, if that second one is at this specified space.
		/// Example: If card to test is at (0, 0) with a radius of 2,
		/// and other is at (6, 6) also with a radius of 2, they don't overlap.
		/// But if overrideOtherSpace is specified as (2, 2), they would overlap.
		/// </summary>
		public IIdentity<Space> overrideOtherSpace;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
			overrideOtherSpace?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			if (card == null) return false;

			var otherCard = other.From(context);
			if (otherCard == null) return false;

			var otherSpace = overrideOtherSpace?.From(context)
				?? otherCard.Position;
			if (otherSpace == null) return false;

			return otherCard.Overlaps(card, otherSpace);
		}
	}
}