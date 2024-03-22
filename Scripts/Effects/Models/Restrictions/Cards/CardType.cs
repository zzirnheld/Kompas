using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class TCard : CardRestrictionBase
	{
		[JsonProperty]
		public char TCard;

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card?.Type == TCard;
	}

	public class Character : TCard
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			TCard = 'C';
			base.Initialize(initializationContext);
		}
	}

	public class Spell : TCard
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			TCard = 'S';
			base.Initialize(initializationContext);
		}
	}

	public class Augment : TCard
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			TCard = 'A';
			base.Initialize(initializationContext);
		}
	}
}