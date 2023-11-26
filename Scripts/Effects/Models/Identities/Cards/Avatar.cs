using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Cards
{

	public class Avatar : ContextualParentIdentityBase<IGameCardInfo>
	{
		public IIdentity<IPlayer> player;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override IGameCardInfo AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
			=> player.From(context, secondaryContext).Avatar;
	}
}