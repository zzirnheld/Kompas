using Kompas.Cards.Models;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Players
{
	public class ControllerOf : ContextualParentIdentityBase<Player>
	{
		[JsonProperty]
		public IIdentity<GameCardBase> card;
		[JsonProperty]
		public IIdentity<IStackable> stackable;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			stackable?.Initialize(initializationContext);

			if (AllNull(card, stackable)) throw new System.ArgumentException($"Must provide something to check controller of");
		}

		protected override Player AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			if (card != null) return card.From(context, secondaryContext).ControllingPlayer;
			if (stackable != null) return stackable.From(context, secondaryContext).ControllingPlayer;
			throw new System.ArgumentException("huh?");
		}
	}
}