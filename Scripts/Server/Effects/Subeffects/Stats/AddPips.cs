using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AddPipsData : SubeffectData { }

public class AddPips : ServerSubeffect
{
	public AddPips(AddPipsData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		GetPlayerTarget(resolution.Context).Pips += AdjustX(resolution.Context);
		return Task.FromResult(ResolutionInfo.Next);
	}
}