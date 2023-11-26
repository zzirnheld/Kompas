using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public abstract class StackableIs : TriggerGamestateRestrictionBase
	{
		[JsonProperty]
		public IIdentity<IStackable> stackable = new Identities.Stackables.StackableCause();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			stackable.Initialize(initializationContext);
		}

		protected override bool IsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> Predicate(stackable.From(context, secondaryContext));

		protected abstract bool Predicate(IStackable? stackable);
	}

	public class IsAttack : StackableIs
	{
		protected override bool Predicate(IStackable? stackable) => stackable is Attack;
	}

	public class IsEffect : StackableIs
	{
		protected override bool Predicate(IStackable? stackable) => stackable is Effect;
	}

	public class Normally : StackableIs
	{
		protected override bool Predicate(IStackable? stackable) => stackable == null;
	}
}