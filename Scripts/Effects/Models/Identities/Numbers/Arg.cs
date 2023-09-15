namespace Kompas.Effects.Models.Identities.Numbers
{
	public class Arg : ContextlessLeafIdentityBase<int>
	{
		protected override int AbstractItem => InitializationContext.subeffect.Effect.arg;
	}
}