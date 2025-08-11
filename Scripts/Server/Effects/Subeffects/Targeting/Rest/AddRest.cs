using System.Threading.Tasks;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AddRestSubeffect : CardTarget
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var (_, targets) = DeterminePossibleTargets(resolution.Context);
		resolution.Context.AddRest(targets);
		return Task.FromResult(ResolutionInfo.Next);
	}
}