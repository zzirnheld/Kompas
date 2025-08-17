using System.Threading.Tasks;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AddRestData : CardTargetData { }

public class AddRest : CardTarget
{
	public AddRest(AddRestData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var (_, targets) = DeterminePossibleTargets(resolution.Context);
		resolution.Context.AddRest(targets);
		return Task.FromResult(ResolutionInfo.Next);
	}
}