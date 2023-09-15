using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Players
{
	public class TriggeringPlayer : TriggerContextualLeafIdentityBase<Player>
	{
		protected override Player AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.player;
	}
}
