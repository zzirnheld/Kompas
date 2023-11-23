using Kompas.Cards.Models;
using Kompas.Effects.Subeffects;
using Kompas.Server.Effects.Models.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class SpaceRestrictionWillBeValid : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public int subeffectIndex;
		#nullable restore

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			_ = InitializationContext.effect ?? throw new System.NullReferenceException("No eff");
			return InitializationContext.effect.Subeffects[subeffectIndex] is SpaceTarget spaceTgtSubeff
					&& spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: card?.Card);
		}
	}
}