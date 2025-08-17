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

public class CardTargetSaveRestData : CardTargetData
{
	[JsonProperty]
	public IRestriction<IGameCardInfo>? restRestriction;
}

public class CardTargetSaveRest : CardTarget
{
	/// <summary>
	/// If null, default to cardRestriction
	/// </summary>
	private readonly IRestriction<IGameCardInfo> restRestriction;

	public CardTargetSaveRest(CardTargetSaveRestData data) : base(data)
	{
		restRestriction = data.restRestriction ?? data.cardRestriction;
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		//May cause duplicate initializations, but that's acceptable
		restRestriction.Initialize(DefaultInitializationContext);
	}

	public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
	{
		base.AdjustSubeffectIndices(increment, startingAtIndex);
		restRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
	}

	protected override Task<ResolutionInfo> NoPossibleTargets(IServerResolutionContext context)
	{
		AddRestTo(context);
		return base.NoPossibleTargets(context);
	}

	protected override void AddList(IEnumerable<GameCard> choices, ServerEffectResolution resolution)
	{
		base.AddList(choices, resolution);
		AddRestTo(resolution.Context, where: cardInfo => !choices.Contains(cardInfo));
	}

	private void AddRestTo(IServerResolutionContext context, Func<IGameCardInfo, bool>? where = null)
	{
		_ = restRestriction ?? throw new NotInitializedException();
		where ??= cardInfo => true;

		var cardsToSearch = toSearch.From(context, context)
			?? throw new InvalidOperationException();
		var rest = cardsToSearch
			.Where(cardInfo => restRestriction.IsValid(cardInfo, context))
			.Where(where)
			.Select(cardInfo => cardInfo.Card);
		context.AddRest(rest);
	}
}