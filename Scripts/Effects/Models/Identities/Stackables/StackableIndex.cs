using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Stackables
{
	public class StackableIndex : EffectContextualLeafIdentityBase<IStackable>
	{
		[JsonProperty]
		public int index = -1;

		protected override IStackable AbstractItemFrom(IResolutionContext toConsider)
			=> EffectHelper.GetItem(toConsider.StackableTargets, index)
			?? throw new IllDefinedException(); 
	}
}