namespace Kompas.Server.Effects.Models.Subeffects;

public class TTimesSubeffect : Loop
{
	public int T;
	private int count = 0;

	protected override void OnLoopExit(IServerResolutionContext context)
	{
		base.OnLoopExit(context);
		count = 0;
	}

    protected override bool LoopContinuation(ServerEffectResolution resolution)
    {
        count++;
        return count < T;
    }
}