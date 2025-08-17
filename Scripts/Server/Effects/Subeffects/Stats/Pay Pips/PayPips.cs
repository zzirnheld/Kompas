using System.Threading.Tasks;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PayPipsData : SubeffectData
{
	[JsonProperty]
	public IIdentity<int> pipCost = new EffectX();
}

public class PayPips : ServerSubeffect
{
	public IIdentity<int> pipCost;

	public PayPips(PayPipsData data) : base(data)
	{
		pipCost = data.pipCost;
	}

	public override bool IsImpossible(IResolutionContext context, TargetingContext? targetingContext = null)
	{
		var player = context.GetPlayerTarget(targetingContext.OrElse(CurrTargetingContext));
		return player is not null && player.Pips < GetToPay(context);
	}

	private int GetToPay(IResolutionContext context)
	{
		return pipCost.From(context, context);
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		pipCost.Initialize(DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		int toPay = GetToPay(resolution.Context);
		var player = GetPlayerTarget(resolution.Context)
			?? throw new NullPlayerException(TargetWasNull);
		if (player.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));

		player.Pips -= toPay;
		return Task.FromResult(ResolutionInfo.Next);
	}
}