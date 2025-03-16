using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetX : ServerSubeffect
{
    public virtual int GetBaseCount(IServerResolutionContext context) => context.X;

    public int GetTrueCount(IServerResolutionContext context)
		=> (GetBaseCount(context) * xMultiplier / xDivisor)
			+ xModifier
			+ (change ? context.X : 0);

    public bool change = false;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.X = GetTrueCount(resolution.Context);
		Logger.Log($"Setting X to {resolution.Context.X}");
		return Task.FromResult(ResolutionInfo.Next);
	}
}