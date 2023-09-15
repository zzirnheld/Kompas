using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Players
{
	public class TargetIndex : ContextlessLeafIdentityBase<Player>
	{
		[JsonProperty]
		public int index = -1;

		protected override Player AbstractItem => InitializationContext.effect.GetPlayer(index);
	}
}