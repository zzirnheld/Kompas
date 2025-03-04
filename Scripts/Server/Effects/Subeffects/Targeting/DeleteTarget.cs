using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DeleteTarget : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.RemoveTarget(GetCardTarget(resolution.Context));
		return Task.FromResult(ResolutionInfo.Next);
	}
}