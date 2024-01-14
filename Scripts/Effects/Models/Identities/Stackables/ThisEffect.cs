using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities.Stackables
{
	public class ThisEffect : ContextlessLeafIdentityBase<IStackable>
	{
		protected override IStackable AbstractItem => InitializationContext.effect
			?? throw new IllDefinedException();
	}
}