namespace Kompas.Effects.Models.InitializationRequirements
{
	public class EffectInitializationRequirement : IInitializationRequirement
	{
		public bool Validate(InitializationContext initializationContext)
		{
			if (initializationContext.effect == null) throw new System.ArgumentNullException($"{GetType()} must be initialized by/with an Effect");

			return true;
		}
	}
}