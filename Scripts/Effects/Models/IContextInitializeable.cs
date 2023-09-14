namespace Kompas.Effects.Models
{
	public interface IContextInitializeable
	{
		public void Initialize(EffectInitializationContext initializationContext);

		/// <summary>
        /// This is separate from the rest of initialization because it can happen at an arbitrary later time,
        /// i.e. when a partial keyword subeffect is expanded and now needs to affect other subeffects
        /// </summary>
        /// <param name="increment">How much the subeffect indices need to be adjusted by</param>
        /// <param name="startingAtIndex">A threshold index for which ones need to be adjusted
        /// (i.e. the starting index of the newly inserted subeffects)</param>
		public void AdjustSubeffectIndices(int increment, int startingAtIndex = 0);
	}
}