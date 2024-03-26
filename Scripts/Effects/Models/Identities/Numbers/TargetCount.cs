using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;
using System.Linq;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class TargetCount : EffectContextualLeafIdentityBase<int>
	{
		[JsonProperty]
		public IRestriction<IGameCardInfo> cardRestriction = new Restrictions.Gamestate.AlwaysValid();

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override int AbstractItemFrom(IResolutionContext toConsider)
		{
			var effect = InitializationContext.effect ?? throw new IllDefinedException();
			return effect.CardTargets
				.Count(c => cardRestriction.IsValid(c, toConsider));
		}
	}
}