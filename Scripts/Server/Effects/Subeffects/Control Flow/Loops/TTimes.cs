namespace Kompas.Server.Effects.Models.Subeffects;

public class TTimesSubeffect : Loop
{
	public int T;
	private int count = 0;

	protected override void OnLoopExit()
	{
		base.OnLoopExit();
		count = 0;
	}

    protected override bool LoopContinuation(ServerEffectResolution resolution)
    {
        count++;
        return count < T;
    }
}