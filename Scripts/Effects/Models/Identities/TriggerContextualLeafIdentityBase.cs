namespace Kompas.Effects.Models.Identities
{
	public abstract class TriggerContextualLeafIdentityBase<ReturnType> : EffectContextualLeafIdentityBase<ReturnType>
	{
		protected override ReturnType AbstractItemFrom(IResolutionContext toConsider)
			=> AbstractItemFrom(toConsider.TriggerContext);

		/// <summary>
		/// Override this one if you ONLY need to know about the context you should actually be considering
		/// </summary>
		/// <param name="contextToConsider">The ActivationContext you actually should be considering.</param>
		protected abstract ReturnType AbstractItemFrom(TriggeringEventContext contextToConsider);
	}
}