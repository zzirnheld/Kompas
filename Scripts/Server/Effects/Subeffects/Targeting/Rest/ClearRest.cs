using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ClearRest : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.Rest.Clear();
		return Task.FromResult(ResolutionInfo.Next);
	}
}