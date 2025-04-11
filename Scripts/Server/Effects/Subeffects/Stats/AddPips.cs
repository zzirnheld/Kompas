using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AddPips : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
        GetPlayerTarget(resolution.Context).Pips += AdjustX(resolution.Context);
		return Task.FromResult(ResolutionInfo.Next);
	}
}