namespace Kompas.Effects.Models.Identities.Stackables
{
	public class StackableCause : TriggerContextualLeafIdentityBase<IStackable>
	{
		protected override IStackable? AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.stackableCause;
	}
	public class StackableEvent : TriggerContextualLeafIdentityBase<IStackable>
	{
		protected override IStackable? AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.stackableEvent;
	}
}