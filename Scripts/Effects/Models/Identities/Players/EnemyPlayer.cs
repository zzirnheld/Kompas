using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Players
{
	public class EnemyPlayer : ContextlessLeafIdentityBase<Player>
	{
		protected override Player AbstractItem => InitializationContext.Controller.Enemy;
	}
}