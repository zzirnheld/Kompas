using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class CardTargetSaveRest : CardTarget
	{
		/// <summary>
		/// If null, default to cardRestriction
		/// </summary>
		[JsonProperty]
		public IRestriction<IGameCardInfo>? restRestriction;

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
			_ = restRestriction ?? throw new NotInitializedException();
			var rest = ServerGame.Cards.Where(c => restRestriction.IsValid(c, ResolutionContext));
			ServerEffect.rest.AddRange(rest);
			return base.NoPossibleTargets();
		}

		protected override void AddList(IEnumerable<IGameCard> choices)
		{
			_ = restRestriction ?? throw new NotInitializedException();
			base.AddList(choices);
			var rest = (toSearch.From(ResolutionContext, ResolutionContext)
				?.Where(c => restRestriction.IsValid(c, ResolutionContext) && !choices.Contains(c))
				.Select(c => c.Card))
				?? throw new InvalidOperationException();
			ServerEffect.rest.AddRange(rest);
		}
	}
}