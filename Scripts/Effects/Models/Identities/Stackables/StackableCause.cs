namespace Kompas.Effects.Models.Identities.Stackables
{
	public class StackableCause : TriggerContextualLeafIdentityBase<IStackable>
	{
		protected override IStackable? AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.StackableCause;
	}
	public class StackableEvent : TriggerContextualLeafIdentityBase<IStackable>
	{
		protected override IStackable? AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.StackableEvent;
	}
}