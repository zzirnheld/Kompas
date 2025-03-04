using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EndResolution : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
	}
}