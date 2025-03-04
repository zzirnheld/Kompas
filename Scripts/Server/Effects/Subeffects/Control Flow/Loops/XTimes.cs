using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class XTimes : Loop
{
	private int count = 0;

    protected override bool LoopContinuation(ServerEffectResolution resolution)
    {
        count++;
        return count < ServerEffect.X;
    }

    protected override void OnLoopExit(IServerResolutionContext context)
	{
		base.OnLoopExit(context);
		count = 0;
	}

	public override Task<ResolutionInfo> OnImpossible(ServerEffectResolution resolution, string why)
	{
		count = 0;
		return base.OnImpossible(resolution, why);
	}
}