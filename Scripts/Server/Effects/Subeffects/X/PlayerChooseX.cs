using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PlayerChooseXData : SubeffectData
{
	[JsonProperty(Required = Required.Always)]
	public IRestriction<int>? XRest;
}

public class PlayerChooseX : ServerSubeffect
{
	private readonly IRestriction<int> xRestriction;

	public PlayerChooseX(PlayerChooseXData data) : base(data)
	{
		xRestriction = data.XRest
			?? throw new MissingJSONValueException(nameof(xRestriction), this);
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		xRestriction.Initialize(DefaultInitializationContext);
	}

	private async Task<int> AskForX(IServerResolutionContext context)
		=> await ServerGame.Awaiter.GetPlayerXValue(GetPlayerTarget(context)
			?? throw new InvalidOperationException("Did you delete a player from the player targets list?"));

	public override async Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		bool xLegal = false;
		while (!xLegal)
		{
			int x = await AskForX(resolution.Context);
			xLegal = SetXIfLegal(x, resolution.Context);
		}
		return ResolutionInfo.Next;
	}

	public bool SetXIfLegal(int x, IServerResolutionContext context)
	{
		if (xRestriction.IsValid(x, context))
		{
			context.X = x;
			return true;
		}
		return false;
	}
}