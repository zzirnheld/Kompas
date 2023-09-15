using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class TargetIndex : ContextlessLeafIdentityBase<Space>
	{
		[JsonProperty]
		public int index = -1;

		protected override Space AbstractItem => InitializationContext.effect.GetSpace(index);
	}
}