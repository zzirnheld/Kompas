using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetXData : SubeffectData
{
	[JsonProperty]
	public bool change = false;
}

public class SetX : ServerSubeffect
{
	private readonly bool change;

	public SetX(SetXData data) : base(data)
	{
		change = data.change;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.X = GetTrueCount(resolution.Context);
		Logger.Log($"Setting X to {resolution.Context.X}");
		return Task.FromResult(ResolutionInfo.Next);
	}

	public virtual int GetBaseCount(IServerResolutionContext context) => context.X;

	public int GetTrueCount(IServerResolutionContext context)
		=> (GetBaseCount(context) * xMultiplier / xDivisor)
			+ xModifier
			+ (change ? context.X : 0);
}