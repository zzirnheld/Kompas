using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ClearRestData : SubeffectData { }

public class ClearRest : ServerSubeffect
{
	public ClearRest(ClearRestData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.Rest.Clear();
		return Task.FromResult(ResolutionInfo.Next);
	}
}