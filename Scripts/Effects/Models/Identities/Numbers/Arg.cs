using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class Arg : ContextlessLeafIdentityBase<int>
	{
        protected override int AbstractItem
        {
            get
            {
				var effect = InitializationContext.effect
					?? throw new IllDefinedException(); //TODO go through all IllDefinedExceptions and add InitializationRequirements
                return effect.arg;
            }
        }
    }
}