namespace Kompas.Effects.Models.InitializationRequirements
{
	public interface IInitializationRequirement
	{
		public bool Validate(InitializationContext initializationContext);
	}
}