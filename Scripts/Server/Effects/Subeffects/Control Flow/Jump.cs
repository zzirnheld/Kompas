using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class JumpData : SubeffectData { }

public class Jump : ServerSubeffect
{
	public Jump(JumpData data) : base(data) { }
	
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		//this will always jump to the given subeffect index
		return Task.FromResult(ResolutionInfo.Index(JumpIndex));
	}
}