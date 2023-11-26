using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class TargetIndex : ContextlessLeafIdentityBase<Space>
	{
		[JsonProperty]
		public int index = -1;

		protected override Space? AbstractItem
		{
			get
			{
				var effect = InitializationContext.effect
					?? throw new IllDefinedException();

				return effect.GetSpace(index);
			}
		}
	}
}