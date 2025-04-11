using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PlayerChooseX : ServerSubeffect
{
	#nullable disable
	[JsonProperty(Required = Required.Always)]
	public IRestriction<int> XRest;
	#nullable restore

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		XRest.Initialize(DefaultInitializationContext);
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
		if (XRest.IsValid(x, context))
		{
			context.X = x;
			return true;
		}
		return false;
	}
}