using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class EffectUsesThisTurn : ContextlessLeafIdentityBase<int>
	{
        protected override int AbstractItem
        {
            get
            {
				var effect = InitializationContext.effect
					?? throw new IllDefinedException();
                return effect.TimesUsedThisTurn;
            }
        }
    }
}