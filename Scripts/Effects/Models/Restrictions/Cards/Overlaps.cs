using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Overlaps : CardRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase> other;
		/// <summary>
		/// An optional override to check if the card to be tested overlaps another, if that second one is at this specified space.
		/// Example: If card to test is at (0, 0) with a radius of 2,
		/// and other is at (6, 6) also with a radius of 2, they don't overlap.
		/// But if overrideOtherSpace is specified as (2, 2), they would overlap.
		/// </summary>
		public IIdentity<Space> overrideOtherSpace;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
			overrideOtherSpace?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
		{
			var otherCard = other.From(context);
			var otherSpace = overrideOtherSpace?.From(context) ?? otherCard.Position;

			return otherCard.Overlaps(card, otherSpace);
		}
	}
}