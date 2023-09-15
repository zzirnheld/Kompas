namespace Kompas.Effects.Models.InitializationRequirements
{
	public class SubeffectInitializationRequirement : IInitializationRequirement
	{
		public bool Validate(EffectInitializationContext initializationContext)
		{
			if (initializationContext.subeffect == null) throw new System.ArgumentNullException($"{GetType()} must be initialized by/with a Subeffect");

			return true;
		}
	}
}