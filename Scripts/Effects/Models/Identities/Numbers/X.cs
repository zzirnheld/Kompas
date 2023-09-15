using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class TriggerX : TriggerContextualLeafIdentityBase<int>
	{
		[JsonProperty]
		public int multiplier = 1;
		[JsonProperty]
		public int modifier = 0;
		[JsonProperty]
		public int divisor = 1;

		protected override int AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> (contextToConsider.x.GetValueOrDefault() * multiplier / divisor) + modifier;
	}

	public class EffectX : EffectContextualLeafIdentityBase<int>
	{
		[JsonProperty]
		public int multiplier = 1;
		[JsonProperty]
		public int modifier = 0;
		[JsonProperty]
		public int divisor = 1;

		protected override int AbstractItemFrom(IResolutionContext contextToConsider)
			=> (contextToConsider.X * multiplier / divisor) + modifier;
	}
}