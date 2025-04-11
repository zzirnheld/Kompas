using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

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

	protected override Task<ResolutionInfo> NoPossibleTargets(IServerResolutionContext context)
	{
		_ = restRestriction ?? throw new NotInitializedException();
		var rest = ServerGame.Cards.Where(c => restRestriction.IsValid(c, context));
		context.AddRest(rest);
		return base.NoPossibleTargets(context);
	}

	protected override void AddList(IEnumerable<GameCard> choices, ServerEffectResolution resolution)
	{
		_ = restRestriction ?? throw new NotInitializedException();
		base.AddList(choices, resolution);
		var rest = (toSearch.From(resolution.Context, resolution.Context)
			?.Where(c => restRestriction.IsValid(c, resolution.Context) && !choices.Contains(c))
			.Select(c => c.Card))
			?? throw new InvalidOperationException();
		resolution.Context.AddRest(rest);
	}
}