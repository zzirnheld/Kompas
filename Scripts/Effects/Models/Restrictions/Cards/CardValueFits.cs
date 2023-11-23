using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities.Numbers;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class CardValueFits : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public CardValue cardValue;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<int> numberRestriction;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardValue.Initialize(initializationContext);
			numberRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic (IGameCardInfo? item, IResolutionContext context)
			=> numberRestriction.IsValid(cardValue.GetValueOf(item), context);
	}

	public class Hurt : CardRestrictionBase
	{
		protected override bool IsValidLogic (IGameCardInfo? item, IResolutionContext context)
			=> item.Hurt;
	}
}