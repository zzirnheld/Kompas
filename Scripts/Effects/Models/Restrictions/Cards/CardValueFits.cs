
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class CardValueFits : CardRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public CardValue cardValue;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<int> numberRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardValue.Initialize(initializationContext);
			numberRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
			=> numberRestriction.IsValid(cardValue.GetValueOf(item), context);
	}

	public class Hurt : CardRestrictionBase
	{
		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
			=> item.Hurt;
	}
}