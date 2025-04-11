using System.Threading.Tasks;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AddRestSubeffect : CardTarget
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.Context.AddRest(DeterminePossibleTargets(resolution.Context));
		return Task.FromResult(ResolutionInfo.Next);
	}
}