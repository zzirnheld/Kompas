using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetX : ServerSubeffect
{
    public virtual int GetBaseCount(IServerResolutionContext context) => Effect.X;

    public int GetTrueCount(IServerResolutionContext context)
		=> (GetBaseCount(context) * xMultiplier / xDivisor)
			+ xModifier
			+ (change ? Effect.X : 0);

    public bool change = false;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.X = GetTrueCount(resolution.Context);
		Logger.Log($"Setting X to {Effect.X}");
		return Task.FromResult(ResolutionInfo.Next);
	}
}