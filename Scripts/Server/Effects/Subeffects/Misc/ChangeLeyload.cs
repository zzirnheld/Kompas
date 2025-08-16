using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ChangeLeyloadData : SubeffectData { }

public class ChangeLeyload : ServerSubeffect
{
	public ChangeLeyload(ChangeLeyloadData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		ServerGame.Leyload += AdjustX(resolution.Context);
		return Task.FromResult(ResolutionInfo.Next);
	}
}