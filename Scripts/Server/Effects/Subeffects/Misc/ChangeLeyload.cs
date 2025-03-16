using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ChangeLeyload : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		ServerGame.Leyload += AdjustX(resolution.Context);
		return Task.FromResult(ResolutionInfo.Next);
	}
}