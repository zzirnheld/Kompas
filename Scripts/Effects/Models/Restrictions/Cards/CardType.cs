using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class CardType : CardRestrictionBase
	{
		[JsonProperty]
		public char cardType;

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card?.Type == cardType;
	}

	public class Character : CardType
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			cardType = 'C';
			base.Initialize(initializationContext);
		}
	}

	public class Spell : CardType
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			cardType = 'S';
			base.Initialize(initializationContext);
		}
	}

	public class Augment : CardType
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			cardType = 'A';
			base.Initialize(initializationContext);
		}
	}
}