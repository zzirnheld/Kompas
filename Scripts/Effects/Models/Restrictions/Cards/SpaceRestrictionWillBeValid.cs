using Kompas.Cards.Models;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class SpaceRestrictionWillBeValid : CardRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public int subeffectIndex;

		protected override bool IsValidLogic(IGameCard card, IResolutionContext context)
			=> InitializationContext.effect.Subeffects[subeffectIndex] is SpaceTarget spaceTgtSubeff
					&& spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: card?.Card);
	}
}