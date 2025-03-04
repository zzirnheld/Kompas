using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AddRestSubeffect : CardTarget
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.rest.AddRange(DeterminePossibleTargets(resolution.Context));
		return Task.FromResult(ResolutionInfo.Next);
	}
}