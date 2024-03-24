using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Identities.Players
{
	public class TriggeringPlayer : TriggerContextualLeafIdentityBase<IPlayer>
	{
		protected override IPlayer? AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> contextToConsider.Player;
	}
}
