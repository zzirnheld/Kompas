using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Cards
{
	public class TargetIndex : EffectContextualCardIdentityBase
	{
		[JsonProperty]
		public int index = -1;

		protected override GameCardBase AbstractItemFrom(IResolutionContext contextToConsider)
		{
			return InitializationContext.effect?.identityOverrides.TargetCardOverride
				?? EffectHelpers.GetItem(contextToConsider.CardTargets, index);
		} 
	}

	public class TargetCardInfoIndex : EffectContextualCardIdentityBase
	{
		[JsonProperty]
		public int index = -1;

		protected override GameCardBase AbstractItemFrom(IResolutionContext contextToConsider)
			=> EffectHelpers.GetItem(contextToConsider.CardInfoTargets, index);
	}
}