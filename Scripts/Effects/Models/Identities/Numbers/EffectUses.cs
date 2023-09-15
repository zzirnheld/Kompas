namespace Kompas.Effects.Models.Identities.Numbers
{
	public class EffectUsesThisTurn : ContextlessLeafIdentityBase<int>
	{
		protected override int AbstractItem => InitializationContext.subeffect.Effect.TimesUsedThisTurn;
	}
}