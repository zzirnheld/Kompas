using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Players
{
	public class TargetIndex : ContextlessLeafIdentityBase<IPlayer>
	{
		[JsonProperty]
		public int index = -1;

		protected override IPlayer AbstractItem => InitializationContext.effect.GetPlayer(index);
	}
}