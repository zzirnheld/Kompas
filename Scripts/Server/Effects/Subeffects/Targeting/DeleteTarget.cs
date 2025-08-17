using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DeleteTargetData : SubeffectData { }

public class DeleteTarget : ServerSubeffect
{
	public DeleteTarget(DeleteTargetData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.RemoveTarget(GetCardTarget(resolution.Context));
		return Task.FromResult(ResolutionInfo.Next);
	}
}