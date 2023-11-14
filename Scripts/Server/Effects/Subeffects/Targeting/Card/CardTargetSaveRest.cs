using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class CardTargetSaveRest : CardTarget
	{
		public IRestriction<IGameCardInfo> restRestriction;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			if (restRestriction == null) restRestriction = cardRestriction;
			else restRestriction.Initialize(DefaultInitializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			restRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override Task<ResolutionInfo> NoPossibleTargets()
		{
			var rest = ServerGame.Cards.Where(c => restRestriction.IsValid(c, ResolutionContext));
			ServerEffect.rest.AddRange(rest);
			return base.NoPossibleTargets();
		}

		protected override void AddList(IEnumerable<GameCard> choices)
		{
			base.AddList(choices);
			var rest = toSearch.From(ResolutionContext, default)
				.Where(c => restRestriction.IsValid(c, ResolutionContext) && !choices.Contains(c))
				.Select(c => c.Card);
			ServerEffect.rest.AddRange(rest);
		}
	}
}