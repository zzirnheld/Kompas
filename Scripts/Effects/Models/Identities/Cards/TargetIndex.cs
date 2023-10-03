using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class TargetIndex : EffectContextualCardIdentityBase
	{
		[JsonProperty]
		public int index = -1;

		protected override IGameCard AbstractItemFrom(IResolutionContext contextToConsider)
		{
			return InitializationContext.effect?.identityOverrides.TargetCardOverride
				?? EffectHelper.GetItem(contextToConsider.CardTargets, index);
		} 
	}

	public class TargetCardInfoIndex : EffectContextualCardIdentityBase
	{
		[JsonProperty]
		public int index = -1;

		protected override IGameCard AbstractItemFrom(IResolutionContext contextToConsider)
			=> EffectHelper.GetItem(contextToConsider.CardInfoTargets, index);
	}
}