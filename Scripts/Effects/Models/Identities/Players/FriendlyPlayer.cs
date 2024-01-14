using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Players
{
	public class FriendlyPlayer : ContextlessLeafIdentityBase<IPlayer>
	{
		protected override IPlayer? AbstractItem => InitializationContext.Owner;
	}
}