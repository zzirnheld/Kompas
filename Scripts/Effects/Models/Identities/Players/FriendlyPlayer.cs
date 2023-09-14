using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Players
{
	public class FriendlyPlayer : ContextlessLeafIdentityBase<Player>
	{
		protected override Player AbstractItem => InitializationContext.Controller;
	}
}