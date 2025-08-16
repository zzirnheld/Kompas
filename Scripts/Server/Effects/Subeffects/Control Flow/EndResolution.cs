using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EndResolutionData : SubeffectData { }

public class EndResolution : ServerSubeffect
{
	public EndResolution(EndResolutionData data) : base(data) { }
	
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
	}
}