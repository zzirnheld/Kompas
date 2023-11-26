using Kompas.Cards.Models;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Players
{
	public class ControllerOf : ContextualParentIdentityBase<IPlayer>
	{
		[JsonProperty]
		public IIdentity<IGameCardInfo> card;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);

			if (AllNull(card)) throw new System.ArgumentException($"Must provide something to check controller of");
		}

		protected override IPlayer AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			if (card != null) return card.From(context, secondaryContext).ControllingPlayer;
			throw new System.ArgumentException("huh?");
		}
	}
}