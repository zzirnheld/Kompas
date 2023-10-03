using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Cards
{

	public class Avatar : ContextualParentIdentityBase<IGameCard>
	{
		public IIdentity<IPlayer> player;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override IGameCard AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> player.From(context, secondaryContext).Avatar;
	}
}