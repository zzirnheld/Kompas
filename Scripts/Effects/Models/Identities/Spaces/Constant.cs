using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class Constant : ContextlessLeafIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public int x;
		[JsonProperty(Required = Required.Always)]
		public int y;

		protected override Space AbstractItem => (x, y);
	}
}